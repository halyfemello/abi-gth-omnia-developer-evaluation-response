using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Users;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public DeleteUserHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingUser == null)
            {
                return DeleteUserResponse.CreateFailure("User not found");
            }

            var userDto = _mapper.Map<UserDto>(existingUser);

            var userInfo = new Dictionary<string, object>
            {
                ["UserId"] = existingUser.Id,
                ["Username"] = existingUser.Username,
                ["Email"] = existingUser.Email,
                ["Role"] = existingUser.Role.ToString(),
                ["Status"] = existingUser.Status.ToString(),
                ["CreatedAt"] = existingUser.CreatedAt
            };

            // Soft delete - desativar usuário em vez de deletar fisicamente
            existingUser.Deactivate();
            await _userRepository.UpdateAsync(existingUser, cancellationToken);

            userDto.Status = "Inactive";

            return DeleteUserResponse.CreateSuccess(userDto);
        }
        catch (Exception ex)
        {
            return DeleteUserResponse.CreateFailure($"Erro ao deletar usuário: {ex.Message}");
        }
    }
}
