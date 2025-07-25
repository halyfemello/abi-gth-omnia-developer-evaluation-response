using AutoMapper;
using DeveloperEvaluation.Core.Application.Queries.Carts;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Carts;

public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, GetCartByIdResponse>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetCartByIdHandler(
        IRepository<Cart> cartRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<GetCartByIdResponse> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid cart ID");
            }

            var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            return _mapper.Map<GetCartByIdResponse>(cart);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
