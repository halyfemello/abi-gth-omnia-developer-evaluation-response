using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Domain.Events;

public class SaleCancelledEvent : INotification
{
    public Sale Sale { get; }
    public DateTime EventDate { get; }
    public Guid EventId { get; }
    public string CancellationReason { get; }

    public SaleCancelledEvent(Sale sale, string cancellationReason = "Venda cancelada pelo usuário")
    {
        Sale = sale;
        CancellationReason = cancellationReason;
        EventDate = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
