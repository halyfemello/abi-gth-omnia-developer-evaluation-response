using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Products;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsResponse>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetAllProductsResponse> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllAsync();

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

            return GetAllProductsResponse.CreateSuccess(productDtos);
        }
        catch (Exception ex)
        {
            return GetAllProductsResponse.CreateFailure($"Erro ao buscar produtos: {ex.Message}");
        }
    }
}
