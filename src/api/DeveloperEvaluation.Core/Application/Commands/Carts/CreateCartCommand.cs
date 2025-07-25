using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Carts;

public class CreateCartCommand : IRequest<CreateCartResponse>
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductDto> Products { get; set; } = new();

    public bool IsValid()
    {
        return UserId > 0 &&
               Date != default &&
               Products != null &&
               Products.All(p => p.ProductId > 0 && p.Quantity > 0);
    }
}

public class CreateCartResponse
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductDto> Products { get; set; } = new();
}
