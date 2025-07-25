using DeveloperEvaluation.Core.Domain.Common;

namespace DeveloperEvaluation.Core.Domain.Entities;

public class Product : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public ProductRating Rating { get; set; } = new ProductRating(0, 0);

    public ProductStatus Status { get; set; } = ProductStatus.Active;

    public int Stock { get; set; }

    protected Product() { }

    public Product(string title, decimal price, string description, string category, string image, ProductRating rating, int stock)
    {
        Title = title;
        Price = price;
        Description = description;
        Category = category;
        Image = image;
        Rating = rating;
        Stock = stock;
        Status = ProductStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCategory(string category)
    {
        Category = category;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImage(string image)
    {
        Image = image;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(ProductRating rating)
    {
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Title) &&
               Title.Length <= 200 &&
               Price > 0 &&
               !string.IsNullOrWhiteSpace(Category) &&
               (string.IsNullOrEmpty(Description) || Description.Length <= 1000) &&
               Stock >= 0;
    }

    public void Activate()
    {
        if (!IsValid())
            throw new InvalidOperationException("Produto deve ser válido para ser ativado");

        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Discontinue()
    {
        Status = ProductStatus.Discontinued;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool IsInStock()
    {
        return Stock > 0;
    }

    public bool IsAvailableForSale()
    {
        return Status == ProductStatus.Active && IsInStock();
    }

    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new ArgumentException("Estoque não pode ser negativo");

        Stock = newStock;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class ProductRating
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
    protected ProductRating() { }

    public ProductRating(decimal rate, int count)
    {
        Rate = rate;
        Count = count;
    }
}

public enum ProductStatus
{
    Active,
    Inactive,
    Discontinued
}
