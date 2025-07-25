FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/api/DeveloperEvaluation.Api/DeveloperEvaluation.Api.csproj", "src/api/DeveloperEvaluation.Api/"]
COPY ["src/api/DeveloperEvaluation.Core/DeveloperEvaluation.Core.csproj", "src/api/DeveloperEvaluation.Core/"]
COPY ["src/api/DeveloperEvaluation.Infra/DeveloperEvaluation.Infra.csproj", "src/api/DeveloperEvaluation.Infra/"]
RUN dotnet restore "./src/api/DeveloperEvaluation.Api/DeveloperEvaluation.Api.csproj"
COPY . .
WORKDIR "/src/src/api/DeveloperEvaluation.Api"
RUN dotnet build "./DeveloperEvaluation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeveloperEvaluation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeveloperEvaluation.Api.dll"]
