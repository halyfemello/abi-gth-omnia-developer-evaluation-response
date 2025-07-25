using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Carts;
using DeveloperEvaluation.Core.Application.Services;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Carts;

public class GetCartsHandler : IRequestHandler<GetCartsQuery, GetCartsResponse>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CartFilterBuilder _filterBuilder;

    public GetCartsHandler(
        IRepository<Cart> cartRepository,
        IMapper mapper,
        IMediator mediator,
        CartFilterBuilder filterBuilder)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _mediator = mediator;
        _filterBuilder = filterBuilder;
    }

    public async Task<GetCartsResponse> Handle(GetCartsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid query parameters");
            }

            if (!_filterBuilder.IsValidFilter(request.UserId, request.MinDate, request.MaxDate))
            {
                throw new ArgumentException("Invalid filter parameters");
            }

            var filter = _filterBuilder.BuildFilter(
                userId: request.UserId,
                minDate: request.MinDate,
                maxDate: request.MaxDate);

            var result = await _cartRepository.GetPagedAsync(
                filter: filter,
                orderBy: request.Order,
                page: request.Page,
                size: request.Size,
                cancellationToken: cancellationToken);

            var cartDtos = _mapper.Map<List<CartDto>>(result.Items);

            var pagedResult = new CartsPagedResponseDto
            {
                Data = cartDtos,
                CurrentPage = request.Page,
                TotalItems = result.TotalCount,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / request.Size)
            };

            return GetCartsResponse.CreateSuccess(pagedResult);
        }
        catch (Exception)
        {

            throw;
        }
    }
}
