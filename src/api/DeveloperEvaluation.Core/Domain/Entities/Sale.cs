using DeveloperEvaluation.Core.Domain.Common;

namespace DeveloperEvaluation.Core.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public List<SaleItem> Items { get; set; } = new();

    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Venda já está cancelada");

        IsCancelled = true;
        CancelledAt = DateTime.UtcNow;

        foreach (var item in Items)
        {
            item.Cancel();
        }

        CalculateTotalAmount();
    }

    public void AddItem(SaleItem item)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Não é possível adicionar itens a uma venda cancelada");

        ValidateItemQuantity(item);

        item.ApplyQuantityDiscount();

        Items.Add(item);
        CalculateTotalAmount();
    }

    public void RemoveItem(Guid itemId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Não é possível remover itens de uma venda cancelada");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            CalculateTotalAmount();
        }
    }

    private void ValidateItemQuantity(SaleItem item)
    {
        if (item.Quantity > 20)
        {
            throw new InvalidOperationException($"Não é possível vender mais de 20 itens idênticos. Produto: {item.ProductName}");
        }

        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId && !i.IsCancelled);
        if (existingItem != null)
        {
            var totalQuantity = existingItem.Quantity + item.Quantity;
            if (totalQuantity > 20)
            {
                throw new InvalidOperationException($"Quantidade total excede o limite de 20 itens. Produto: {item.ProductName}");
            }
        }
    }
}
