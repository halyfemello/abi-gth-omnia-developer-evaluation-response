using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Users;

public class CreateUserCommand : IRequest<CreateUserResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserNameDto Name { get; set; } = new();
    public UserAddressDto? Address { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string Role { get; set; } = "Customer";

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Username) &&
               Username.Length >= 3 &&
               !string.IsNullOrWhiteSpace(Password) &&
               Password.Length >= 6 &&
               Name != null &&
               !string.IsNullOrWhiteSpace(Name.FirstName) &&
               !string.IsNullOrWhiteSpace(Name.LastName);
    }
}

public class CreateUserResponse
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

    public static CreateUserResponse CreateSuccess(UserDto user)
    {
        return new CreateUserResponse
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
            Message = "Usuário criado com sucesso"
        };
    }

    public static CreateUserResponse CreateFailure(string message)
    {
        return new CreateUserResponse
        {
            Success = false,
            Message = message
        };
    }
}
