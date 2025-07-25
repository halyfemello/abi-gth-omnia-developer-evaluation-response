using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Carts;

public class GetCartByIdQuery : IRequest<GetCartByIdResponse>
{
    public Guid Id { get; set; }

    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class GetCartByIdResponse
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductDto> Products { get; set; } = new();
}
