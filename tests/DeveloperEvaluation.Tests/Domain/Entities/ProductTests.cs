using DeveloperEvaluation.Core.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DeveloperEvaluation.Tests.Domain.Entities;

/// <summary>
/// Testes unitários para a entidade Product
/// Valida regras de negócio e comportamentos do domínio
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Testa criação de produto válido
    /// Regra de negócio: Produto deve ser criado com status Active por padrão
    /// </summary>
    [Fact(DisplayName = "🟢 Deve criar produto com dados válidos")]
    public void CreateProduct_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var title = "Smartphone Samsung Galaxy";
        var price = 1500.99m;
        var description = "Smartphone com 128GB de armazenamento";
        var category = "Electronics";
        var image = "https://example.com/image.jpg";
        var rating = new ProductRating(4.5m, 150);
        var stock = 25;

        // Act
        var product = new Product(title, price, description, category, image, rating, stock);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().NotBeEmpty();
        product.Title.Should().Be(title);
        product.Price.Should().Be(price);
        product.Description.Should().Be(description);
        product.Category.Should().Be(category);
        product.Image.Should().Be(image);
        product.Rating.Should().Be(rating);
        product.Stock.Should().Be(stock);
        product.Status.Should().Be(ProductStatus.Active);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa validação de produto com dados válidos
    /// Regra de negócio: Produto com dados válidos deve ser considerado válido
    /// </summary>
    [Fact(DisplayName = "🟢 Deve validar produto com dados válidos")]
    public void IsValid_WithValidProduct_ShouldReturnTrue()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    /// <summary>
    /// Testa validação de produto com título vazio
    /// Regra de negócio: Título é obrigatório
    /// </summary>
    [Theory(DisplayName = "🔴 Deve falhar validação com título inválido")]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValid_WithInvalidTitle_ShouldReturnFalse(string invalidTitle)
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateTitle(invalidTitle);

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Testa validação de produto com título nulo
    /// Regra de negócio: Título é obrigatório
    /// </summary>
    [Fact(DisplayName = "🔴 Deve falhar validação com título nulo")]
    public void IsValid_WithNullTitle_ShouldReturnFalse()
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateTitle(null!);

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Testa validação de produto com preço inválido
    /// Regra de negócio: Preço deve ser maior que 0
    /// </summary>
    [Theory(DisplayName = "🔴 Deve falhar validação com preço inválido")]
    [InlineData(0)]
    [InlineData(-10.5)]
    [InlineData(-1)]
    public void IsValid_WithInvalidPrice_ShouldReturnFalse(decimal invalidPrice)
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdatePrice(invalidPrice);

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Testa validação de produto com categoria vazia
    /// Regra de negócio: Categoria é obrigatória
    /// </summary>
    [Theory(DisplayName = "🔴 Deve falhar validação com categoria inválida")]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValid_WithInvalidCategory_ShouldReturnFalse(string invalidCategory)
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateCategory(invalidCategory);

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Testa validação de produto com categoria nula
    /// Regra de negócio: Categoria é obrigatória
    /// </summary>
    [Fact(DisplayName = "🔴 Deve falhar validação com categoria nula")]
    public void IsValid_WithNullCategory_ShouldReturnFalse()
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateCategory(null!);

        // Act
        var isValid = product.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    /// <summary>
    /// Testa validação de produto com estoque negativo
    /// Regra de negócio: UpdateStock deve lançar exceção para valores negativos
    /// </summary>
    [Theory(DisplayName = "🔴 Deve lançar exceção ao atualizar estoque com valor negativo")]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void UpdateStock_WithNegativeStock_ShouldThrowException(int negativeStock)
    {
        // Arrange
        var product = CreateValidProduct();

        // Act & Assert
        var action = () => product.UpdateStock(negativeStock);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Estoque não pode ser negativo");
    }

    /// <summary>
    /// Testa ativação de produto
    /// Regra de negócio: Produto pode ser ativado quando inativo
    /// </summary>
    [Fact(DisplayName = "🟢 Deve ativar produto inativo com sucesso")]
    public void Activate_InactiveProduct_ShouldActivateSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        product.Status.Should().Be(ProductStatus.Active);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa desativação de produto
    /// Regra de negócio: Produto pode ser desativado quando ativo
    /// </summary>
    [Fact(DisplayName = "🟢 Deve desativar produto ativo com sucesso")]
    public void Deactivate_ActiveProduct_ShouldDeactivateSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.Deactivate();

        // Assert
        product.Status.Should().Be(ProductStatus.Inactive);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa descontinuação de produto
    /// Regra de negócio: Produto pode ser descontinuado
    /// </summary>
    [Fact(DisplayName = "🟢 Deve descontinuar produto com sucesso")]
    public void Discontinue_Product_ShouldDiscontinueSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.Discontinue();

        // Assert
        product.Status.Should().Be(ProductStatus.Discontinued);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa atualização de estoque
    /// Regra de negócio: Estoque pode ser atualizado para valores não negativos
    /// </summary>
    [Theory(DisplayName = "🟢 Deve atualizar estoque com valores válidos")]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void UpdateStock_WithValidStock_ShouldUpdateSuccessfully(int newStock)
    {
        // Arrange
        var product = CreateValidProduct();

        // Act
        product.UpdateStock(newStock);

        // Assert
        product.Stock.Should().Be(newStock);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa atualização de avaliação
    /// Regra de negócio: Avaliação pode ser atualizada
    /// </summary>
    [Fact(DisplayName = "🟢 Deve atualizar avaliação do produto com sucesso")]
    public void UpdateRating_WithValidRating_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = CreateValidProduct();
        var newRating = new ProductRating(3.8m, 75);

        // Act
        product.UpdateRating(newRating);

        // Assert
        product.Rating.Should().Be(newRating);
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Testa verificação se produto está em estoque
    /// Regra de negócio: Produto está em estoque se Stock > 0
    /// </summary>
    [Theory(DisplayName = "🟢 Deve verificar se produto está em estoque corretamente")]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(0, false)]
    public void IsInStock_WithDifferentStockLevels_ShouldReturnCorrectResult(int stock, bool expectedResult)
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateStock(stock);

        // Act
        var isInStock = product.IsInStock();

        // Assert
        isInStock.Should().Be(expectedResult);
    }

    /// <summary>
    /// Testa verificação se produto está disponível para venda
    /// Regra de negócio: Produto está disponível se estiver ativo e em estoque
    /// </summary>
    [Theory(DisplayName = "🟢 Deve verificar disponibilidade para venda corretamente")]
    [InlineData(ProductStatus.Active, 1, true)]
    [InlineData(ProductStatus.Active, 0, false)]
    [InlineData(ProductStatus.Inactive, 1, false)]
    [InlineData(ProductStatus.Discontinued, 1, false)]
    public void IsAvailableForSale_WithDifferentConditions_ShouldReturnCorrectResult(
        ProductStatus status, int stock, bool expectedResult)
    {
        // Arrange
        var product = CreateValidProduct();
        product.UpdateStock(stock);

        switch (status)
        {
            case ProductStatus.Inactive:
                product.Deactivate();
                break;
            case ProductStatus.Discontinued:
                product.Discontinue();
                break;
        }

        // Act
        var isAvailable = product.IsAvailableForSale();

        // Assert
        isAvailable.Should().Be(expectedResult);
    }

    /// <summary>
    /// Método auxiliar para criar um produto válido para testes
    /// </summary>
    private static Product CreateValidProduct()
    {
        return new Product(
            "Produto Teste",
            99.99m,
            "Descrição do produto teste",
            "Categoria Teste",
            "https://example.com/image.jpg",
            new ProductRating(4.0m, 50),
            10
        );
    }
}
