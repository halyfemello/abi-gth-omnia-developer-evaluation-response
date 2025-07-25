using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Products;

public class DeleteProductCommand : IRequest<DeleteProductResponse>
{
    public Guid Id { get; set; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }

    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class DeleteProductResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static DeleteProductResponse CreateSuccess()
    {
        return new DeleteProductResponse
        {
            Success = true,
            Message = "Produto deletado com sucesso"
        };
    }

    public static DeleteProductResponse CreateFailure(string message)
    {
        return new DeleteProductResponse
        {
            Success = false,
            Message = message
        };
    }
}
