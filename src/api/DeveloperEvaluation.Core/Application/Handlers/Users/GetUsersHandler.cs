using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Users;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;
using DeveloperEvaluation.Core.Application.Services;

namespace DeveloperEvaluation.Core.Application.Handlers.Users;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly UserFilterBuilder _filterBuilder;

    public GetUsersHandler(
        IRepository<User> userRepository,
        IMapper mapper,
        IMediator mediator,
        UserFilterBuilder filterBuilder)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
        _filterBuilder = filterBuilder;
    }

    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filter = _filterBuilder.BuildFilter(
                email: request.Email,
                username: request.Username,
                status: request.Status,
                role: request.Role,
                firstName: request.FirstName,
                lastName: request.LastName,
                city: request.City);

            var result = await _userRepository.GetPagedAsync(
                filter: filter,
                orderBy: request.Order,
                page: request.Page,
                size: request.Size,
                cancellationToken: cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(result.Items);

            var paginatedResult = new PagedResultDto<UserDto>
            {
                Data = userDtos,
                CurrentPage = request.Page,
                PageSize = request.Size,
                TotalItems = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.Size)
            };

            return GetUsersResponse.CreateSuccess(paginatedResult);
        }
        catch (Exception ex)
        {
            return GetUsersResponse.CreateFailure($"Erro ao recuperar usuários: {ex.Message}");
        }
    }
}
