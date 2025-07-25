using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Users;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateUserHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                request.Email.ToLower() != existingUser.Email.ToLower())
            {
                var (emailUsers, _) = await _userRepository.GetPagedAsync(
                    u => u.Email.ToLower() == request.Email.ToLower() && u.Id != request.Id,
                    null, 1, 1, cancellationToken);

                if (emailUsers.Any())
                {
                    throw new ArgumentException("A user with this email already exists");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Username) &&
                request.Username.ToLower() != existingUser.Username.ToLower())
            {
                var (usernameUsers, _) = await _userRepository.GetPagedAsync(
                    u => u.Username.ToLower() == request.Username.ToLower() && u.Id != request.Id,
                    null, 1, 1, cancellationToken);

                if (usernameUsers.Any())
                {
                    throw new ArgumentException("A user with this username already exists");
                }
            }

            var oldValues = new Dictionary<string, object>
            {
                ["Email"] = existingUser.Email,
                ["Username"] = existingUser.Username,
                ["FirstName"] = existingUser.Name.FirstName,
                ["LastName"] = existingUser.Name.LastName,
                ["Role"] = existingUser.Role.ToString(),
                ["Status"] = existingUser.Status.ToString()
            };

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                existingUser.UpdateEmail(request.Email);
            }

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                existingUser.UpdateUsername(request.Username);
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var hashedPassword = HashPassword(request.Password);
                existingUser.UpdatePasswordHash(hashedPassword);
            }

            if (request.Name != null &&
                (!string.IsNullOrWhiteSpace(request.Name.FirstName) || !string.IsNullOrWhiteSpace(request.Name.LastName)))
            {
                var firstName = !string.IsNullOrWhiteSpace(request.Name.FirstName)
                    ? request.Name.FirstName
                    : existingUser.Name.FirstName;

                var lastName = !string.IsNullOrWhiteSpace(request.Name.LastName)
                    ? request.Name.LastName
                    : existingUser.Name.LastName;

                var newName = new UserName(firstName, lastName);
                existingUser.UpdateName(newName);
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                existingUser.UpdatePhone(request.Phone);
            }

            if (request.Address != null &&
                (!string.IsNullOrWhiteSpace(request.Address.Street) ||
                !string.IsNullOrWhiteSpace(request.Address.City) ||
                !string.IsNullOrWhiteSpace(request.Address.ZipCode) ||
                request.Address.Number > 0 ||
                request.Address.GeoLocation != null))
            {
                var geoLocation = request.Address.GeoLocation != null
                    ? new GeoLocation(
                        request.Address.GeoLocation.Latitude ?? "0",
                        request.Address.GeoLocation.Longitude ?? "0")
                    : existingUser.Address?.GeoLocation;

                var address = new UserAddress(
                    request.Address.City ?? existingUser.Address?.City ?? string.Empty,
                    request.Address.Street ?? existingUser.Address?.Street ?? string.Empty,
                    request.Address.Number > 0 ? request.Address.Number : existingUser.Address?.Number ?? 0,
                    request.Address.ZipCode ?? existingUser.Address?.ZipCode ?? string.Empty,
                    geoLocation);

                existingUser.UpdateAddress(address);
            }

            if (!string.IsNullOrWhiteSpace(request.Role) &&
                Enum.TryParse<UserRole>(request.Role, true, out var userRole))
            {
                existingUser.UpdateRole(userRole);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (string.Equals(request.Status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    existingUser.Activate();
                }
                else if (string.Equals(request.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                {
                    existingUser.Deactivate();
                }
            }

            await _userRepository.UpdateAsync(existingUser, cancellationToken);

            return new UpdateUserResponse
            {
                Id = existingUser.Id,
                Email = existingUser.Email,
                Username = existingUser.Username,
                Password = "***", // Não retornar a senha real
                Name = _mapper.Map<UserNameDto>(existingUser.Name),
                Address = existingUser.Address != null ? _mapper.Map<UserAddressDto>(existingUser.Address) : null,
                Phone = existingUser.Phone,
                Status = existingUser.Status.ToString(),
                Role = existingUser.Role.ToString()
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
