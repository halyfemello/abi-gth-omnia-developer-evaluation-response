using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class CancelSaleItemCommand : IRequest<bool>
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }

    public CancelSaleItemCommand(Guid saleId, Guid itemId)
    {
        SaleId = saleId;
        ItemId = itemId;
    }
}
