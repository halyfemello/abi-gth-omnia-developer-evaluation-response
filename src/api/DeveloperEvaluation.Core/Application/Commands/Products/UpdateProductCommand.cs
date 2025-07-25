using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Products;

public class UpdateProductCommand : IRequest<UpdateProductResponse>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public ProductRatingDto? Rating { get; set; }
    public int Stock { get; set; }

    public bool IsValid()
    {
        return Id != Guid.Empty &&
               !string.IsNullOrWhiteSpace(Title) &&
               Price > 0 &&
               !string.IsNullOrWhiteSpace(Category) &&
               Stock >= 0;
    }
}

public class UpdateProductResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public ProductRatingDto Rating { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public int Stock { get; set; }
}
