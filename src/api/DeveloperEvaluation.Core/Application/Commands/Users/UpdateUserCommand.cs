using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Users;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public Guid Id { get; set; }
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
        return Id != Guid.Empty &&
               !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Username) &&
               Username.Length >= 3 &&
               !string.IsNullOrWhiteSpace(Password) &&
               Password.Length >= 6 &&
               Name != null &&
               !string.IsNullOrWhiteSpace(Name.FirstName) &&
               !string.IsNullOrWhiteSpace(Name.LastName);
    }
}

public class UpdateUserResponse
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
}
