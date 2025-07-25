using AutoMapper;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Products;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return GetProductByIdResponse.CreateNotFound();
            }

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return GetProductByIdResponse.CreateNotFound();
            }

            var productDto = new ProductDto
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
            };

            return GetProductByIdResponse.CreateSuccess(productDto);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Erro ao buscar produto: {ex.Message}", ex);
        }
    }
}
