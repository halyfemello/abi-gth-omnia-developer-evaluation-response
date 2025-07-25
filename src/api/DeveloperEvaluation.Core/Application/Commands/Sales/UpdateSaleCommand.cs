using MediatR;
using DeveloperEvaluation.Core.Application.DTOs;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class UpdateSaleCommand : IRequest<SaleDto>
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<CreateSaleItemDto> Items { get; set; } = new();
}
