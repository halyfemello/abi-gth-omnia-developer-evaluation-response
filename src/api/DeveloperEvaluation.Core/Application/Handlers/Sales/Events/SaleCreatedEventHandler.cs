using MediatR;
using Microsoft.Extensions.Logging;
using DeveloperEvaluation.Core.Domain.Events;

namespace DeveloperEvaluation.Core.Application.Handlers.Sales.Events;

public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Aqui você pode adicionar integração com message broker, métricas, etc.
        // Por exemplo: await _messagePublisher.PublishAsync("sales.created", notification);
        // Porem, apenas logamos o evento

        _logger.LogInformation(
            "🔥 [SALE_CREATED] - EventId: {EventId} | SaleId: {SaleId} | SaleNumber: {SaleNumber} | Customer: {CustomerName} ({CustomerId}) | TotalAmount: ${TotalAmount:F2} | ItemsCount: {ItemsCount} | Timestamp: {Timestamp}",
            notification.EventId,
            notification.Sale.Id,
            notification.Sale.SaleNumber,
            notification.Sale.CustomerName,
            notification.Sale.CustomerId,
            notification.Sale.TotalAmount,
            notification.Sale.Items.Count(i => !i.IsCancelled),
            notification.EventDate
        );

        return Task.CompletedTask;
    }
}
