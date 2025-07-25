using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Products;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Dados inválidos para criação do produto");
            }

            var rating = request.Rating != null
                ? new ProductRating(request.Rating.Rate, request.Rating.Count)
                : new ProductRating(0, 0);

            var product = new Product(
                request.Title,
                request.Price,
                request.Description,
                request.Category,
                request.Image,
                rating,
                request.Stock
            );

            if (!product.IsValid())
            {
                throw new ArgumentException("Produto não atende às regras de negócio");
            }

            await _productRepository.CreateAsync(product);

            return new CreateProductResponse
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
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Erro ao criar produto: {ex.Message}", ex);
        }
    }
}
