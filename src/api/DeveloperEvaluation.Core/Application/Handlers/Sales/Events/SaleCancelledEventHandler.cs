using MediatR;
using Microsoft.Extensions.Logging;
using DeveloperEvaluation.Core.Domain.Events;

namespace DeveloperEvaluation.Core.Application.Handlers.Sales.Events;

public class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        // Aqui você pode adicionar integração com sistemas de estorno, message broker, notificações, etc.
        // Por exemplo: await _refundService.ProcessRefundAsync(notification.Sale);
        // Porem, apenas logamos o evento

        _logger.LogWarning(
            "🚫 [SALE_CANCELLED] - EventId: {EventId} | SaleId: {SaleId} | SaleNumber: {SaleNumber} | Customer: {CustomerName} | OriginalAmount: ${OriginalAmount:F2} | Reason: {CancellationReason} | CancelledAt: {CancelledAt} | Timestamp: {Timestamp}",
            notification.EventId,
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.TotalAmount,
            notification.CancellationReason,
            notification.Sale.CancelledAt,
            notification.EventDate
        );


        return Task.CompletedTask;
    }
}
