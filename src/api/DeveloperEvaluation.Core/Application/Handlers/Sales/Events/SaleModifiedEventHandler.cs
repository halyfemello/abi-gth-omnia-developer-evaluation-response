using MediatR;
using Microsoft.Extensions.Logging;
using DeveloperEvaluation.Core.Domain.Events;

namespace DeveloperEvaluation.Core.Application.Handlers.Sales.Events;

public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        // Aqui você pode adicionar integração com message broker, auditoria, etc.
        // Por exemplo: await _auditService.LogModificationAsync(notification);
        // Porem, apenas logamos o evento

        _logger.LogInformation(
            "✏️ [SALE_MODIFIED] - EventId: {EventId} | SaleId: {SaleId} | SaleNumber: {SaleNumber} | Customer: {CustomerName} | TotalAmount: ${TotalAmount:F2} | Modification: {ModificationDescription} | Timestamp: {Timestamp}",
            notification.EventId,
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.TotalAmount,
            notification.ModificationDescription,
            notification.EventDate
        );

        return Task.CompletedTask;
    }
}
