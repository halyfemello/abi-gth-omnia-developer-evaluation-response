using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Domain.Events;

public class ItemCancelledEvent : INotification
{
    public Sale Sale { get; }
    public SaleItem CancelledItem { get; }
    public DateTime EventDate { get; }
    public Guid EventId { get; }
    public string CancellationReason { get; }

    public ItemCancelledEvent(Sale sale, SaleItem cancelledItem, string cancellationReason = "Item cancelado pelo usuário")
    {
        Sale = sale;
        CancelledItem = cancelledItem;
        CancellationReason = cancellationReason;
        EventDate = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
