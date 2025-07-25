using System.ComponentModel.DataAnnotations;

namespace DeveloperEvaluation.Core.Application.DTOs;

public class SalesQueryParametersDto
{
    [Range(1, int.MaxValue, ErrorMessage = "O número da página deve ser maior que 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100")]
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? SaleNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor mínimo deve ser maior ou igual a 0")]
    public decimal? MinTotalAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor máximo deve ser maior ou igual a 0")]
    public decimal? MaxTotalAmount { get; set; }
    public string? Status { get; set; }

    public bool IsValidDateRange()
    {
        if (MinSaleDate.HasValue && MaxSaleDate.HasValue)
        {
            return MinSaleDate.Value <= MaxSaleDate.Value;
        }
        return true;
    }
    public bool IsValidAmountRange()
    {
        if (MinTotalAmount.HasValue && MaxTotalAmount.HasValue)
        {
            return MinTotalAmount.Value <= MaxTotalAmount.Value;
        }
        return true;
    }
}
