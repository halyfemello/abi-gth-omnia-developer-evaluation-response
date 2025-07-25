using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Domain.Events;

public class SaleModifiedEvent : INotification
{
    public Sale Sale { get; }
    public DateTime EventDate { get; }
    public Guid EventId { get; }
    public string ModificationDescription { get; }

    public SaleModifiedEvent(Sale sale, string modificationDescription = "Venda atualizada")
    {
        Sale = sale;
        ModificationDescription = modificationDescription;
        EventDate = DateTime.UtcNow;
        EventId = Guid.NewGuid();
    }
}
