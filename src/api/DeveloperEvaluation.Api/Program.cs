using DeveloperEvaluation.Core.Application.Mappings;
using DeveloperEvaluation.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DeveloperEvaluation.Api.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Developer Evaluation API",
        Version = "v1",
        Description = "API para sistema de vendas com arquitetura DDD e CQRS"
    });

    if (builder.Environment.IsProduction())
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Por favor insira um token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    }
});

// MongoDB Configuration
builder.Services.AddMongoDb(builder.Configuration);

// AutoMapper
builder.Services.AddAutoMapper(typeof(SaleMappingProfile), typeof(DeveloperEvaluation.Core.Application.Mappings.ProductMappingProfile), typeof(DeveloperEvaluation.Core.Application.Mappings.UserMappingProfile), typeof(DeveloperEvaluation.Core.Application.Mappings.CartMappingProfile), typeof(DeveloperEvaluation.Core.Application.Mappings.AuthMappingProfile));

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(DeveloperEvaluation.Core.Application.Commands.Sales.CreateSaleCommand).Assembly);
});

// Application Services
builder.Services.AddScoped<DeveloperEvaluation.Core.Application.Services.SaleFilterBuilder>();
builder.Services.AddScoped<DeveloperEvaluation.Core.Application.Services.ProductFilterBuilder>();
builder.Services.AddScoped<DeveloperEvaluation.Core.Application.Services.UserFilterBuilder>();
builder.Services.AddScoped<DeveloperEvaluation.Core.Application.Services.CartFilterBuilder>();
builder.Services.AddScoped<DeveloperEvaluation.Core.Application.Services.JwtTokenService>();

// JWT Authentication Configuration
var jwtSecretKey = "DeveloperEvaluation2024SecretKeyForJWTTokenGeneration123456789";
var jwtIssuer = "DeveloperEvaluation.Api";
var jwtAudience = "DeveloperEvaluation.Client";

if (builder.Environment.IsProduction())
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}
else
{
    // Fake Authentication para desenvolvimento
    builder.Services.AddAuthentication("Fake")
        .AddScheme<FakeAuthOptions, FakeAuthHandler>("Fake", options => { });
}

builder.Services.AddAuthorization();

// CORS (para desenvolvimento)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Developer Evaluation API v1");
        c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
    });
}

// Middleware global para tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Comentando o seeder do Entity Framework, pois agora usamos MongoDB
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     DatabaseSeeder.SeedData(context);
// }

app.Run();

// Torna a classe Program pública para testes E2E
public partial class Program { }
