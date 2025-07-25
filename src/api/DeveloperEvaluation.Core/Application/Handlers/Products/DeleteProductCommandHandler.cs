using DeveloperEvaluation.Core.Application.Commands.Products;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Products;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
{
    private readonly IRepository<Product> _productRepository;

    public DeleteProductCommandHandler(IRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return DeleteProductResponse.CreateFailure("ID do produto é inválido");
            }

            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                return DeleteProductResponse.CreateFailure("Produto não encontrado");
            }

            await _productRepository.DeleteAsync(request.Id);

            return DeleteProductResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return DeleteProductResponse.CreateFailure($"Erro ao deletar produto: {ex.Message}");
        }
    }
}
