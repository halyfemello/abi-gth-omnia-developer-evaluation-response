using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Users;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
    public Guid Id { get; set; }

    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class GetUserByIdResponse
{
    public UserDto? User { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetUserByIdResponse CreateSuccess(UserDto user)
    {
        return new GetUserByIdResponse
        {
            User = user,
            Success = true,
            Message = "Usuário encontrado com sucesso"
        };
    }

    public static GetUserByIdResponse CreateNotFound()
    {
        return new GetUserByIdResponse
        {
            User = null,
            Success = false,
            Message = "Usuário não encontrado"
        };
    }

    public static GetUserByIdResponse CreateFailure(string message)
    {
        return new GetUserByIdResponse
        {
            User = null,
            Success = false,
            Message = message
        };
    }
}
