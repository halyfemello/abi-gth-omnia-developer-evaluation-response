using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Products;

public class GetProductsQuery : IRequest<GetProductsResponse>
{
    private int _size = 10;

    public int Page { get; set; } = 1;

    public int Size
    {
        get => _size;
        set => _size = Math.Min(value, 100);
    }

    public string? Order { get; set; }
    public string? Title { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Status { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }

    public bool IsValid()
    {
        if (Page < 1 || Size < 1)
            return false;

        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            return false;

        if (MinStock.HasValue && MaxStock.HasValue && MinStock > MaxStock)
            return false;

        return true;
    }

    public int GetOffset()
    {
        return (Page - 1) * Size;
    }
}

public class GetProductsResponse
{
    public PagedResultDto<ProductDto> Data { get; set; } = new();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static GetProductsResponse CreateSuccess(PagedResultDto<ProductDto> pagedResult)
    {
        return new GetProductsResponse
        {
            Data = pagedResult,
            Success = true,
            Message = "Produtos obtidos com sucesso"
        };
    }

    public static GetProductsResponse CreateFailure(string message)
    {
        return new GetProductsResponse
        {
            Data = new PagedResultDto<ProductDto>(),
            Success = false,
            Message = message
        };
    }
}
