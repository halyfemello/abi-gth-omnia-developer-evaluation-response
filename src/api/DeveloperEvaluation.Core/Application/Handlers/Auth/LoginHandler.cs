using DeveloperEvaluation.Core.Application.Commands.Auth;
using DeveloperEvaluation.Core.Application.Services;
using DeveloperEvaluation.Core.Domain.Events;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace DeveloperEvaluation.Core.Application.Handlers.Auth;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly IMediator _mediator;

    public LoginHandler(
        IRepository<User> userRepository,
        JwtTokenService jwtTokenService,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mediator = mediator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                await PublishLoginFailedEvent("system", "Invalid login data", new { Username = request.Username }, cancellationToken);
                return LoginResponse.CreateFailure("Dados de login inválidos");
            }

            var (users, _) = await _userRepository.GetPagedAsync(
                filter: u => u.Username.ToLower() == request.Username.ToLower(),
                page: 1,
                size: 1,
                cancellationToken: cancellationToken);

            var user = users.FirstOrDefault();
            if (user == null)
            {
                await PublishLoginFailedEvent("system", "User not found", new { Username = request.Username }, cancellationToken);
                return LoginResponse.CreateFailure("Usuário ou senha inválidos");
            }

            var hashedPassword = HashPassword(request.Password);
            if (user.PasswordHash != hashedPassword)
            {
                await PublishLoginFailedEvent(user.Id.ToString(), "Invalid password", new { Username = request.Username }, cancellationToken);
                return LoginResponse.CreateFailure("Usuário ou senha inválidos");
            }

            if (user.Status != UserStatus.Active)
            {
                await PublishLoginFailedEvent(user.Id.ToString(), "User is not active", new { Username = request.Username, Status = user.Status.ToString() }, cancellationToken);
                return LoginResponse.CreateFailure("Usuário inativo");
            }

            var token = _jwtTokenService.GenerateToken(user.Id, user.Username, user.Role.ToString());

            return LoginResponse.CreateSuccess(token, user.Id, user.Role.ToString());
        }
        catch (Exception ex)
        {
            await PublishLoginFailedEvent("system", $"Error during login: {ex.Message}", new { Username = request.Username, Error = ex.Message }, cancellationToken);
            return LoginResponse.CreateFailure("Erro interno do servidor");
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private async Task PublishLoginFailedEvent(string userId, string details, object metadata, CancellationToken cancellationToken)
    {
        // Publica evento de falha de login        
    }
}
