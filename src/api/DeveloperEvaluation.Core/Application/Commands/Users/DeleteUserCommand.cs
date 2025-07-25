using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Users;

public class DeleteUserCommand : IRequest<DeleteUserResponse>
{
    public Guid Id { get; set; }

    public DeleteUserCommand(Guid id)
    {
        Id = id;
    }

    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class DeleteUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto? Address { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static DeleteUserResponse CreateSuccess(UserDto user)
    {
        return new DeleteUserResponse
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Password = user.Password,
            Name = user.Name,
            Address = user.Address,
            Phone = user.Phone,
            Status = user.Status,
            Role = user.Role,
            Success = true,
            Message = "Usuário deletado com sucesso"
        };
    }

    public static DeleteUserResponse CreateFailure(string message)
    {
        return new DeleteUserResponse
        {
            Success = false,
            Message = message
        };
    }
}
