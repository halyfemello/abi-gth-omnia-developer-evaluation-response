using MediatR;
using Microsoft.Extensions.Logging;
using DeveloperEvaluation.Core.Domain.Events;

namespace DeveloperEvaluation.Core.Application.Handlers.Sales.Events;

public class ItemCancelledEventHandler : INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<ItemCancelledEventHandler> _logger;

    public ItemCancelledEventHandler(ILogger<ItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        // Aqui você pode adicionar integração com sistemas de estorno, message broker, notificações, etc.
        // Por exemplo: await _inventoryService.AdjustStockAsync(notification.CancelledItem);
        // Porem, apenas logamos o evento

        _logger.LogWarning(
            "❌ [ITEM_CANCELLED] - EventId: {EventId} | SaleId: {SaleId} | ItemId: {ItemId} | Product: {ProductName} ({ProductId}) | Quantity: {Quantity} | UnitPrice: ${UnitPrice:F2} | ItemTotal: ${ItemTotal:F2} | Reason: {CancellationReason} | Timestamp: {Timestamp}",
            notification.EventId,
            notification.Sale.Id,
            notification.CancelledItem.Id,
            notification.CancelledItem.ProductName,
            notification.CancelledItem.ProductId,
            notification.CancelledItem.Quantity,
            notification.CancelledItem.UnitPrice,
            notification.CancelledItem.TotalAmount,
            notification.CancellationReason,
            notification.EventDate
        );

        return Task.CompletedTask;
    }
}
