using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Carts;

public class DeleteCartCommand : IRequest<DeleteCartResponse>
{
    public Guid Id { get; set; }

    public bool IsValid()
    {
        return Id != Guid.Empty;
    }
}

public class DeleteCartResponse
{
    public string Message { get; set; } = string.Empty;
}
