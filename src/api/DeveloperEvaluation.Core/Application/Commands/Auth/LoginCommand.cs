using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Auth;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Username) &&
               Username.Length >= 3 &&
               !string.IsNullOrWhiteSpace(Password) &&
               Password.Length >= 6;
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserRole { get; set; }

    public static LoginResponse CreateSuccess(string token, Guid userId, string userRole)
    {
        return new LoginResponse
        {
            Token = token,
            Success = true,
            Message = "Login realizado com sucesso",
            UserId = userId,
            UserRole = userRole
        };
    }

    public static LoginResponse CreateFailure(string message)
    {
        return new LoginResponse
        {
            Token = string.Empty,
            Success = false,
            Message = message,
            UserId = null,
            UserRole = null
        };
    }
}
