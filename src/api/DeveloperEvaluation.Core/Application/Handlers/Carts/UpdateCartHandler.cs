using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Carts;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Carts;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResponse>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateCartHandler(
        IRepository<Cart> cartRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<UpdateCartResponse> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid cart data");
            }

            var existingCart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            var oldValues = new Dictionary<string, object>
            {
                ["UserId"] = existingCart.UserId,
                ["Date"] = existingCart.Date,
                ["ProductCount"] = existingCart.Products.Count
            };

            existingCart.UpdateUserId(request.UserId);
            existingCart.UpdateDate(request.Date);

            existingCart.Clear();
            foreach (var product in request.Products)
            {
                existingCart.AddProduct(product.ProductId, product.Quantity);
            }

            await _cartRepository.UpdateAsync(existingCart, cancellationToken);

            var newValues = new Dictionary<string, object>
            {
                ["UserId"] = existingCart.UserId,
                ["Date"] = existingCart.Date,
                ["ProductCount"] = existingCart.Products.Count
            };

            return _mapper.Map<UpdateCartResponse>(existingCart);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
