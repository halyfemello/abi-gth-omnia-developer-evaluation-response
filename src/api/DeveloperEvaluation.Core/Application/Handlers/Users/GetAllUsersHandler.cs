using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Users;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetAllUsersHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync(page: 1, size: 1000, cancellationToken: cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return GetAllUsersResponse.CreateSuccess(userDtos);
        }
        catch (Exception ex)
        {
            return GetAllUsersResponse.CreateFailure($"Erro ao recuperar usuários: {ex.Message}");
        }
    }
}
