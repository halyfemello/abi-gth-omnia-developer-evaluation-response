using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Users;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Events;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateUserHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return CreateUserResponse.CreateFailure("Invalid user data");
            }

            var existingUsersByEmail = await _userRepository.GetPagedAsync(
                filter: u => u.Email.ToLower() == request.Email.ToLower(),
                page: 1,
                size: 1,
                cancellationToken: cancellationToken);

            if (existingUsersByEmail.TotalCount > 0)
            {
                return CreateUserResponse.CreateFailure("A user with this email already exists");
            }

            var existingUsersByUsername = await _userRepository.GetPagedAsync(
                filter: u => u.Username.ToLower() == request.Username.ToLower(),
                page: 1,
                size: 1,
                cancellationToken: cancellationToken);

            if (existingUsersByUsername.TotalCount > 0)
            {
                return CreateUserResponse.CreateFailure("A user with this username already exists");
            }

            var userName = new UserName(request.Name.FirstName, request.Name.LastName);

            UserAddress? address = null;
            if (request.Address != null && !string.IsNullOrWhiteSpace(request.Address.Street))
            {
                var geoLocation = new GeoLocation(
                    request.Address.GeoLocation?.Latitude ?? "0",
                    request.Address.GeoLocation?.Longitude ?? "0");

                address = new UserAddress(
                    request.Address.City,
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.ZipCode,
                    geoLocation);
            }

            var hashedPassword = HashPassword(request.Password);

            if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            {
                userRole = UserRole.Customer;
            }

            var user = new User(
                request.Email,
                request.Username,
                hashedPassword,
                userName,
                request.Phone,
                address,
                userRole);

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

            // Mapear para DTO
            var userDto = _mapper.Map<UserDto>(createdUser);

            // Aqui e nós demais metodos igual o sales você pode adicionar integração com sistemas message broker, notificações, etc.
            // Igual criamos nos eventos de sales

            return CreateUserResponse.CreateSuccess(userDto);
        }
        catch (Exception ex)
        {
            // Aqui e nós demais metodos igual o sales você pode adicionar integração com sistemas message broker, notificações, etc.
            // Igual criamos nos eventos de sales

            return CreateUserResponse.CreateFailure($"Erro ao criar usuário: {ex.Message}");
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
