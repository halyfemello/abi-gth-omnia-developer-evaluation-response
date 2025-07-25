using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Products;

public class GetProductByIdQuery : IRequest<GetProductByIdResponse>
{
    public Guid Id { get; set; }

    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }

    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class GetProductByIdResponse
{
    public ProductDto? Product { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetProductByIdResponse CreateSuccess(ProductDto product)
    {
        return new GetProductByIdResponse
        {
            Product = product,
            Success = true,
            Message = "Produto encontrado com sucesso"
        };
    }

    public static GetProductByIdResponse CreateNotFound()
    {
        return new GetProductByIdResponse
        {
            Product = null,
            Success = false,
            Message = "Produto não encontrado"
        };
    }
}
