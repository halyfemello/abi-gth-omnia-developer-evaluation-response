using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Domain.Events;

public class SaleCreatedEvent : INotification
{
    public Sale Sale { get; }
    public DateTime EventDate { get; }
    public Guid EventId { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
        EventDate = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
