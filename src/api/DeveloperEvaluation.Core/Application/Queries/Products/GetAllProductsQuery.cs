using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Products;


public class GetAllProductsQuery : IRequest<GetAllProductsResponse>
{
    // Query simples sem parâmetros para endpoint /products
}

public class GetAllProductsResponse
{
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetAllProductsResponse CreateSuccess(IList<ProductDto> products)
    {
        return new GetAllProductsResponse
        {
            Products = products,
            Success = true,
            Message = "Produtos obtidos com sucesso"
        };
    }

    public static GetAllProductsResponse CreateFailure(string message)
    {
        return new GetAllProductsResponse
        {
            Products = new List<ProductDto>(),
            Success = false,
            Message = message
        };
    }
}
