using System.ComponentModel.DataAnnotations;

namespace DeveloperEvaluation.Core.Application.DTOs;

public class CartDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "User ID é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID deve ser maior que 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    public List<CartProductDto> Products { get; set; } = new();
}

public class CreateCartDto
{
    [Required(ErrorMessage = "User ID é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID deve ser maior que 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    public List<CartProductDto> Products { get; set; } = new();
}

public class UpdateCartDto
{
    [Required(ErrorMessage = "User ID é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "User ID deve ser maior que 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime Date { get; set; }

    public List<CartProductDto> Products { get; set; } = new();
}

public class CartProductDto
{
    [Required(ErrorMessage = "Product ID é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID deve ser maior que 0")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que 0")]
    public int Quantity { get; set; }
}

public class CartsPagedResponseDto
{
    public List<CartDto> Data { get; set; } = new();

    public long TotalItems { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }
}
