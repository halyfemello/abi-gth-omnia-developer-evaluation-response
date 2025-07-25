using DeveloperEvaluation.Core.Domain.Entities;
using FluentAssertions;

namespace DeveloperEvaluation.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para entidade Sale
/// Valida regras de negócio e comportamentos da venda
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "🟢 Deve calcular valor total da venda corretamente")]
    public void Sale_Should_Calculate_Total_Amount_Correctly()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = "João Silva",
            CustomerEmail = "joao@email.com",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Central"
        };

        var item1 = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto A",
            UnitPrice = 10.00m,
            Quantity = 2
        };

        var item2 = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto B",
            UnitPrice = 15.00m,
            Quantity = 3
        };

        // Act
        sale.AddItem(item1);
        sale.AddItem(item2);

        // Assert
        sale.TotalAmount.Should().Be(65.00m); // (10*2) + (15*3) = 20 + 45 = 65
        sale.Items.Should().HaveCount(2);
    }

    [Fact(DisplayName = "🟢 Deve aplicar desconto de 10% para 4-9 itens")]
    public void Sale_Should_Apply_10_Percent_Discount_For_4_To_9_Items()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-002",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Maria Santos"
        };

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto C",
            UnitPrice = 100.00m,
            Quantity = 5 // 5 itens = 10% desconto
        };

        // Act
        sale.AddItem(item);

        // Assert
        item.DiscountPercentage.Should().Be(10.00m);
        item.TotalAmount.Should().Be(450.00m); // 100 * 5 * 0.9 = 450
        sale.TotalAmount.Should().Be(450.00m);
    }

    [Fact(DisplayName = "🟢 Deve aplicar desconto de 20% para 10-20 itens")]
    public void Sale_Should_Apply_20_Percent_Discount_For_10_To_20_Items()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-003",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Pedro Oliveira"
        };

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto D",
            UnitPrice = 50.00m,
            Quantity = 15 // 15 itens = 20% desconto
        };

        // Act
        sale.AddItem(item);

        // Assert
        item.DiscountPercentage.Should().Be(20.00m);
        item.TotalAmount.Should().Be(600.00m); // 50 * 15 * 0.8 = 600
        sale.TotalAmount.Should().Be(600.00m);
    }

    [Fact(DisplayName = "🔴 Deve lançar exceção ao adicionar mais de 20 itens")]
    public void Sale_Should_Throw_Exception_When_Adding_More_Than_20_Items()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-004",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Ana Costa"
        };

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto E",
            UnitPrice = 10.00m,
            Quantity = 25 // Mais de 20 itens não é permitido
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.AddItem(item));
        exception.Message.Should().Contain("Não é possível vender mais de 20 itens idênticos");
    }

    [Fact(DisplayName = "🟢 Não deve aplicar desconto para menos de 4 itens")]
    public void Sale_Should_Not_Apply_Discount_For_Less_Than_4_Items()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-005",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Carlos Lima"
        };

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto F",
            UnitPrice = 20.00m,
            Quantity = 3 // Menos de 4 itens = sem desconto
        };

        // Act
        sale.AddItem(item);

        // Assert
        item.DiscountPercentage.Should().Be(0.00m);
        item.TotalAmount.Should().Be(60.00m); // 20 * 3 = 60
        sale.TotalAmount.Should().Be(60.00m);
    }

    [Fact(DisplayName = "🟢 Deve cancelar venda com sucesso")]
    public void Sale_Should_Cancel_Successfully()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-006",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Lucia Ferreira"
        };

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto G",
            UnitPrice = 30.00m,
            Quantity = 2
        };

        sale.AddItem(item);

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.CancelledAt.Should().NotBeNull();
        sale.Items.All(i => i.IsCancelled).Should().BeTrue();
        sale.TotalAmount.Should().Be(0.00m); // Total zerado após cancelamento
    }

    [Fact(DisplayName = "🔴 Deve lançar exceção ao adicionar item a venda cancelada")]
    public void Sale_Should_Throw_Exception_When_Adding_Item_To_Cancelled_Sale()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-007",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Roberto Santos"
        };

        sale.Cancel(); // Cancelar venda primeiro

        var item = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Produto H",
            UnitPrice = 25.00m,
            Quantity = 1
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.AddItem(item));
        exception.Message.Should().Contain("Não é possível adicionar itens a uma venda cancelada");
    }
}
