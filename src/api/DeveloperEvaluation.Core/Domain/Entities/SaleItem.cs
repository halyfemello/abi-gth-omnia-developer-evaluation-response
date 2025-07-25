using DeveloperEvaluation.Core.Domain.Common;

namespace DeveloperEvaluation.Core.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid SaleId { get; set; }

    public Sale Sale { get; set; } = null!;

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string ProductDescription { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountPercentage { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public bool IsCancelled { get; private set; }

    public DateTime? CancelledAt { get; private set; }

    public void ApplyQuantityDiscount()
    {
        if (Quantity >= 10 && Quantity <= 20)
        {
            DiscountPercentage = 20m;
        }
        else if (Quantity >= 4)
        {
            DiscountPercentage = 10m;
        }
        else
        {
            DiscountPercentage = 0m;
        }

        CalculateAmounts();
    }

    private void CalculateAmounts()
    {
        var subtotal = Quantity * UnitPrice;
        DiscountAmount = subtotal * (DiscountPercentage / 100);
        TotalAmount = subtotal - DiscountAmount;
    }

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Item já está cancelado");

        IsCancelled = true;
        CancelledAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Não é possível atualizar quantidade de um item cancelado");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        if (newQuantity > 20)
            throw new ArgumentException("Não é possível vender mais de 20 itens idênticos");

        Quantity = newQuantity;
        ApplyQuantityDiscount();
    }

    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Não é possível atualizar preço de um item cancelado");

        if (newUnitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero");

        UnitPrice = newUnitPrice;
        CalculateAmounts();
    }
}
