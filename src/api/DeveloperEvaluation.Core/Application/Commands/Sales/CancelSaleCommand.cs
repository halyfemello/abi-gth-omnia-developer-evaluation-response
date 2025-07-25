using MediatR;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class CancelSaleCommand : IRequest<bool>
{
    public Guid SaleId { get; set; }

    public CancelSaleCommand(Guid saleId)
    {
        SaleId = saleId;
    }
}
