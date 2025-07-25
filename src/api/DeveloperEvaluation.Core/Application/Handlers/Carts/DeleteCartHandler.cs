using DeveloperEvaluation.Core.Application.Commands.Carts;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Carts;

public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResponse>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMediator _mediator;

    public DeleteCartHandler(
        IRepository<Cart> cartRepository,
        IMediator mediator)
    {
        _cartRepository = cartRepository;
        _mediator = mediator;
    }

    public async Task<DeleteCartResponse> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid cart ID");
            }

            var existingCart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            var deleted = await _cartRepository.DeleteAsync(request.Id, cancellationToken);
            if (!deleted)
            {
                throw new InvalidOperationException("Failed to delete cart");
            }

            return new DeleteCartResponse
            {
                Message = "Cart deleted successfully"
            };
        }
        catch (Exception)
        {
            throw;
        }
    }
}
