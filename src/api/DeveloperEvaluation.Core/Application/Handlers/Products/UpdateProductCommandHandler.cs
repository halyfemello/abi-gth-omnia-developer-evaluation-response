using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Products;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Dados inválidos para atualização do produto");
            }

            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                throw new ArgumentException("Produto não encontrado");
            }

            existingProduct.UpdateTitle(request.Title);
            existingProduct.UpdatePrice(request.Price);
            existingProduct.UpdateDescription(request.Description);
            existingProduct.UpdateCategory(request.Category);
            existingProduct.UpdateImage(request.Image);
            existingProduct.UpdateStock(request.Stock);

            if (request.Rating != null)
            {
                var newRating = new ProductRating(request.Rating.Rate, request.Rating.Count);
                existingProduct.UpdateRating(newRating);
            }

            if (!existingProduct.IsValid())
            {
                throw new ArgumentException("Produto atualizado não atende às regras de negócio");
            }

            await _productRepository.UpdateAsync(existingProduct);

            return new UpdateProductResponse
            {
                Id = existingProduct.Id,
                Title = existingProduct.Title,
                Price = existingProduct.Price,
                Description = existingProduct.Description,
                Category = existingProduct.Category,
                Image = existingProduct.Image,
                Rating = new ProductRatingDto
                {
                    Rate = existingProduct.Rating.Rate,
                    Count = existingProduct.Rating.Count
                },
                Status = existingProduct.Status.ToString(),
                Stock = existingProduct.Stock
            };
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Erro ao atualizar produto: {ex.Message}", ex);
        }
    }
}
