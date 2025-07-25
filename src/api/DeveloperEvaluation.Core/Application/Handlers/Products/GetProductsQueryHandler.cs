using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Products;
using DeveloperEvaluation.Core.Application.Services;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly ProductFilterBuilder _filterBuilder;

    public GetProductsQueryHandler(
        IRepository<Product> productRepository,
        IMapper mapper,
        ProductFilterBuilder filterBuilder)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _filterBuilder = filterBuilder;
    }

    public async Task<GetProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return GetProductsResponse.CreateFailure("Parâmetros de consulta inválidos");
            }

            var filter = _filterBuilder.BuildFilter(
                title: request.Title,
                category: request.Category,
                minPrice: request.MinPrice,
                maxPrice: request.MaxPrice,
                status: request.Status,
                minStock: request.MinStock,
                maxStock: request.MaxStock
            );

            var orderBy = BuildOrderString(request.Order);

            var result = await _productRepository.GetPagedAsync(
                filter: filter,
                orderBy: orderBy,
                page: request.Page,
                size: request.Size
            );

            var products = result.Items;
            var totalCount = result.TotalCount;

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                Description = product.Description,
                Category = product.Category,
                Image = product.Image,
                Rating = new ProductRatingDto
                {
                    Rate = product.Rating.Rate,
                    Count = product.Rating.Count
                },
                Status = product.Status.ToString(),
                Stock = product.Stock
            }).ToList();

            var pagedResult = new PagedResultDto<ProductDto>(
                productDtos,
                request.Page,
                request.Size,
                totalCount
            );

            return GetProductsResponse.CreateSuccess(pagedResult);
        }
        catch (Exception ex)
        {
            return GetProductsResponse.CreateFailure($"Erro ao buscar produtos: {ex.Message}");
        }
    }

    private string? BuildOrderString(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return "title asc";
        }

        var allowedFields = new[] { "title", "price", "category", "status", "stock", "rating", "createdat", "updatedat" };
        var orderParts = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var validParts = new List<string>();

        foreach (var part in orderParts)
        {
            var trimmedPart = part.Trim();
            var components = trimmedPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (components.Length == 0) continue;

            var fieldName = components[0].ToLower();
            var direction = components.Length > 1 && components[1].ToLower() == "desc" ? "desc" : "asc";

            if (fieldName == "name")
                fieldName = "title";

            if (fieldName == "title")
                fieldName = "Title";

            if (allowedFields.Contains(fieldName.ToLower()) || fieldName == "Title")
            {
                validParts.Add($"{fieldName} {direction}");
            }
        }

        return validParts.Any() ? string.Join(", ", validParts) : "title asc";
    }
}
