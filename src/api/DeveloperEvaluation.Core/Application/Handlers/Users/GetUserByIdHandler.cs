using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Users;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetUserByIdHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return GetUserByIdResponse.CreateFailure("Invalid query parameters");
            }

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user == null)
            {
                return GetUserByIdResponse.CreateNotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);

            return GetUserByIdResponse.CreateSuccess(userDto);
        }
        catch (Exception ex)
        {
            return GetUserByIdResponse.CreateFailure($"Erro ao recuperar usuário: {ex.Message}");
        }
    }
}
