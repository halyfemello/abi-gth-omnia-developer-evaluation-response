using System.ComponentModel.DataAnnotations;

namespace DeveloperEvaluation.Core.Application.DTOs;

public class ProductDto
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

public class CreateProductDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título não pode ter mais de 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public decimal Price { get; set; }

    [MaxLength(1000, ErrorMessage = "Descrição não pode ter mais de 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public string Category { get; set; } = string.Empty;

    [Url(ErrorMessage = "Imagem deve ser uma URL válida")]
    public string Image { get; set; } = string.Empty;

    public ProductRatingDto? Rating { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
    public int Stock { get; set; }
}

public class UpdateProductDto
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(200, ErrorMessage = "Título não pode ter mais de 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preço é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que 0")]
    public decimal Price { get; set; }

    [MaxLength(1000, ErrorMessage = "Descrição não pode ter mais de 1000 caracteres")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public string Category { get; set; } = string.Empty;

    [Url(ErrorMessage = "Imagem deve ser uma URL válida")]
    public string Image { get; set; } = string.Empty;

    public ProductRatingDto? Rating { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
    public int Stock { get; set; }
}

public class ProductRatingDto
{
    [Range(0, 5, ErrorMessage = "Nota deve estar entre 0 e 5")]
    public decimal Rate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantidade de avaliações não pode ser negativa")]
    public int Count { get; set; }
}

public class ProductsQueryParametersDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
    public int Size { get; set; } = 10;

    public string? Order { get; set; }

    public string? Title { get; set; }

    public string? Category { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Preço mínimo deve ser >= 0")]
    public decimal? MinPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Preço máximo deve ser >= 0")]
    public decimal? MaxPrice { get; set; }
    public string? Status { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo deve ser >= 0")]
    public int? MinStock { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque máximo deve ser >= 0")]
    public int? MaxStock { get; set; }

    public bool IsValidPriceRange()
    {
        if (MinPrice.HasValue && MaxPrice.HasValue)
        {
            return MinPrice.Value <= MaxPrice.Value;
        }
        return true;
    }

    public bool IsValidStockRange()
    {
        if (MinStock.HasValue && MaxStock.HasValue)
        {
            return MinStock.Value <= MaxStock.Value;
        }
        return true;
    }
}
