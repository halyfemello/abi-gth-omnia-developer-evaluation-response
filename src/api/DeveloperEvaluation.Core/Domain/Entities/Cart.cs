using DeveloperEvaluation.Core.Domain.Common;

namespace DeveloperEvaluation.Core.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProduct> Products { get; set; } = new();
    protected Cart() { }
    public Cart(int userId, DateTime date, List<CartProduct>? products = null)
    {
        UserId = userId;
        Date = date;
        Products = products ?? new List<CartProduct>();
    }

    public bool IsValid()
    {
        return UserId > 0 &&
               Date != default &&
               Products != null &&
               Products.All(p => p.IsValid());
    }

    public void AddProduct(int productId, int quantity)
    {
        if (productId <= 0)
            throw new ArgumentException("O ID do produto deve ser maior que 0", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que 0", nameof(quantity));

        var existingProduct = Products.FirstOrDefault(p => p.ProductId == productId);
        if (existingProduct != null)
        {
            existingProduct.UpdateQuantity(quantity);
        }
        else
        {
            Products.Add(new CartProduct(productId, quantity));
        }

        Date = DateTime.UtcNow;
    }

    public void RemoveProduct(int productId)
    {
        var product = Products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            Products.Remove(product);
            Date = DateTime.UtcNow;
        }
    }

    public void UpdateProductQuantity(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            RemoveProduct(productId);
            return;
        }

        var product = Products.FirstOrDefault(p => p.ProductId == productId);
        if (product != null)
        {
            product.UpdateQuantity(quantity);
            Date = DateTime.UtcNow;
        }
    }

    public void Clear()
    {
        Products.Clear();
        Date = DateTime.UtcNow;
    }

    public int GetTotalQuantity()
    {
        return Products.Sum(p => p.Quantity);
    }

    public void UpdateDate(DateTime date)
    {
        Date = date;
    }

    public void UpdateUserId(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("O ID do usuário deve ser maior que 0", nameof(userId));

        UserId = userId;
        Date = DateTime.UtcNow;
    }
}

public class CartProduct
{

    public int ProductId { get; set; }
    public int Quantity { get; set; }
    protected CartProduct() { }
    public CartProduct(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
    public bool IsValid()
    {
        return ProductId > 0 && Quantity > 0;
    }
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que 0", nameof(quantity));

        Quantity = quantity;
    }
}
