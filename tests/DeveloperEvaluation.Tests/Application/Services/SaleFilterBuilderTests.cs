using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Services;
using DeveloperEvaluation.Core.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DeveloperEvaluation.Tests.Application.Services;

/// <summary>
/// Testes unitários para SaleFilterBuilder
/// Valida a aplicação correta das regras de negócio de filtragem
/// </summary>
public class SaleFilterBuilderTests
{
    private readonly SaleFilterBuilder _filterBuilder;

    public SaleFilterBuilderTests()
    {
        _filterBuilder = new SaleFilterBuilder();
    }
    [Fact(DisplayName = "🟢 Deve retornar null quando nenhum parâmetro for fornecido")]
    public void BuildFilter_Should_Return_Null_When_No_Parameters_Provided()
    {
        // Arrange - Configura cenário sem parâmetros de filtro
        var parameters = new SalesQueryParametersDto();

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica que não há filtro quando não há parâmetros
        filter.Should().BeNull("porque não foram fornecidos parâmetros de filtro");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por número da venda com correspondência exata")]
    public void BuildFilter_Should_Filter_By_SaleNumber_Exact_Match()
    {
        // Arrange - Configura filtro por número da venda exato
        var parameters = new SalesQueryParametersDto
        {
            SaleNumber = "SALE-001"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica se aplica filtro exato por número da venda
        filter.Should().NotBeNull("porque foi fornecido número da venda");

        // Teste com dados simulados para validar a expressão
        var testSale = new Sale { SaleNumber = "SALE-001" };
        var compiledFilter = filter!.Compile();
        compiledFilter(testSale).Should().BeTrue("porque o número da venda coincide exatamente");

        var testSaleOther = new Sale { SaleNumber = "SALE-002" };
        compiledFilter(testSaleOther).Should().BeFalse("porque o número da venda não coincide");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por número da venda com wildcard no início")]
    public void BuildFilter_Should_Filter_By_SaleNumber_With_Wildcard_Start()
    {
        // Arrange - Configura filtro com wildcard no início (termina com)
        var parameters = new SalesQueryParametersDto
        {
            SaleNumber = "*001"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para wildcard no início
        filter.Should().NotBeNull("porque foi fornecido padrão com wildcard");

        var testSale = new Sale { SaleNumber = "SALE-001" };
        var compiledFilter = filter!.Compile();
        compiledFilter(testSale).Should().BeTrue("porque a venda termina com '001'");

        var testSaleOther = new Sale { SaleNumber = "SALE-002" };
        compiledFilter(testSaleOther).Should().BeFalse("porque a venda não termina com '001'");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por número da venda com wildcard no final")]
    public void BuildFilter_Should_Filter_By_SaleNumber_With_Wildcard_End()
    {
        // Arrange - Configura filtro com wildcard no fim (começa com)
        var parameters = new SalesQueryParametersDto
        {
            SaleNumber = "SALE*"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para wildcard no fim
        filter.Should().NotBeNull("porque foi fornecido padrão com wildcard");

        var testSale = new Sale { SaleNumber = "SALE-001" };
        var compiledFilter = filter!.Compile();
        compiledFilter(testSale).Should().BeTrue("porque a venda começa com 'SALE'");

        var testSaleOther = new Sale { SaleNumber = "ORDER-001" };
        compiledFilter(testSaleOther).Should().BeFalse("porque a venda não começa com 'SALE'");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por número da venda com wildcard em ambos os lados")]
    public void BuildFilter_Should_Filter_By_SaleNumber_With_Wildcard_Both()
    {
        // Arrange - Configura filtro com wildcard em ambos os lados (contém)
        var parameters = new SalesQueryParametersDto
        {
            SaleNumber = "*001*"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para wildcard duplo
        filter.Should().NotBeNull("porque foi fornecido padrão com wildcard");

        var testSale = new Sale { SaleNumber = "SALE-001-A" };
        var compiledFilter = filter!.Compile();
        compiledFilter(testSale).Should().BeTrue("porque a venda contém '001'");

        var testSaleOther = new Sale { SaleNumber = "SALE-002-A" };
        compiledFilter(testSaleOther).Should().BeFalse("porque a venda não contém '001'");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por ID do cliente")]
    public void BuildFilter_Should_Filter_By_CustomerId()
    {
        // Arrange - Configura filtro por ID do cliente
        var customerId = Guid.NewGuid();
        var parameters = new SalesQueryParametersDto
        {
            CustomerId = customerId
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para filtro por ID do cliente
        filter.Should().NotBeNull("porque foi fornecido ID do cliente");

        var testSale = new Sale { CustomerId = customerId };
        var compiledFilter = filter!.Compile();
        compiledFilter(testSale).Should().BeTrue("porque o ID do cliente coincide");

        var testSaleOther = new Sale { CustomerId = Guid.NewGuid() };
        compiledFilter(testSaleOther).Should().BeFalse("porque o ID do cliente não coincide");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por intervalo de datas")]
    public void BuildFilter_Should_Filter_By_Date_Range()
    {
        // Arrange - Configura filtro por range de data
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        var parameters = new SalesQueryParametersDto
        {
            MinSaleDate = minDate,
            MaxSaleDate = maxDate
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para range de data (inclusive)
        filter.Should().NotBeNull("porque foi fornecido range de data");

        var compiledFilter = filter!.Compile();

        // Teste data dentro do range
        var testSaleInRange = new Sale { SaleDate = new DateTime(2024, 6, 15) };
        compiledFilter(testSaleInRange).Should().BeTrue("porque a data está dentro do range");

        // Teste data no limite mínimo (inclusive)
        var testSaleMinBoundary = new Sale { SaleDate = minDate };
        compiledFilter(testSaleMinBoundary).Should().BeTrue("porque a data mínima é inclusiva");

        // Teste data no limite máximo (inclusive)
        var testSaleMaxBoundary = new Sale { SaleDate = maxDate };
        compiledFilter(testSaleMaxBoundary).Should().BeTrue("porque a data máxima é inclusiva");

        // Teste data fora do range
        var testSaleOutOfRange = new Sale { SaleDate = new DateTime(2023, 12, 31) };
        compiledFilter(testSaleOutOfRange).Should().BeFalse("porque a data está fora do range");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por intervalo de valores")]
    public void BuildFilter_Should_Filter_By_Amount_Range()
    {
        // Arrange - Configura filtro por range de valor
        var minAmount = 100m;
        var maxAmount = 1000m;
        var parameters = new SalesQueryParametersDto
        {
            MinTotalAmount = minAmount,
            MaxTotalAmount = maxAmount
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para range de valor (inclusive)
        filter.Should().NotBeNull("porque foi fornecido range de valor");

        var compiledFilter = filter!.Compile();

        // Teste valor dentro do range - criamos uma venda com item para calcular total
        var testSaleInRange = new Sale();
        testSaleInRange.AddItem(new SaleItem { Quantity = 1, UnitPrice = 500m });
        testSaleInRange.CalculateTotalAmount();
        compiledFilter(testSaleInRange).Should().BeTrue("porque o valor está dentro do range");

        // Teste valor no limite mínimo (inclusive)
        var testSaleMinBoundary = new Sale();
        testSaleMinBoundary.AddItem(new SaleItem { Quantity = 1, UnitPrice = minAmount });
        testSaleMinBoundary.CalculateTotalAmount();
        compiledFilter(testSaleMinBoundary).Should().BeTrue("porque o valor mínimo é inclusivo");

        // Teste valor no limite máximo (inclusive)
        var testSaleMaxBoundary = new Sale();
        testSaleMaxBoundary.AddItem(new SaleItem { Quantity = 1, UnitPrice = maxAmount });
        testSaleMaxBoundary.CalculateTotalAmount();
        compiledFilter(testSaleMaxBoundary).Should().BeTrue("porque o valor máximo é inclusivo");

        // Teste valor fora do range
        var testSaleOutOfRange = new Sale();
        testSaleOutOfRange.AddItem(new SaleItem { Quantity = 1, UnitPrice = 50m });
        testSaleOutOfRange.CalculateTotalAmount();
        compiledFilter(testSaleOutOfRange).Should().BeFalse("porque o valor está fora do range");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por status ativo")]
    public void BuildFilter_Should_Filter_By_Status_Active()
    {
        // Arrange - Configura filtro por status ativo
        var parameters = new SalesQueryParametersDto
        {
            Status = "active"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para status ativo
        filter.Should().NotBeNull("porque foi fornecido status");

        var compiledFilter = filter!.Compile();

        // Teste venda ativa (não cancelada por padrão)
        var testActiveSale = new Sale();
        compiledFilter(testActiveSale).Should().BeTrue("porque a venda está ativa (não cancelada)");

        // Teste venda cancelada
        var testCancelledSale = new Sale();
        testCancelledSale.AddItem(new SaleItem { Quantity = 1, UnitPrice = 100m }); // Precisa ter item para cancelar
        testCancelledSale.Cancel();
        compiledFilter(testCancelledSale).Should().BeFalse("porque a venda está cancelada");
    }

    [Fact(DisplayName = "🟢 Deve filtrar por status cancelado")]
    public void BuildFilter_Should_Filter_By_Status_Cancelled()
    {
        // Arrange - Configura filtro por status cancelado
        var parameters = new SalesQueryParametersDto
        {
            Status = "cancelled"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica aplicação da regra de negócio para status cancelado
        filter.Should().NotBeNull("porque foi fornecido status");

        var compiledFilter = filter!.Compile();

        // Teste venda cancelada
        var testCancelledSale = new Sale();
        testCancelledSale.AddItem(new SaleItem { Quantity = 1, UnitPrice = 100m }); // Precisa ter item para cancelar
        testCancelledSale.Cancel();
        compiledFilter(testCancelledSale).Should().BeTrue("porque a venda está cancelada");

        // Teste venda ativa
        var testActiveSale = new Sale();
        compiledFilter(testActiveSale).Should().BeFalse("porque a venda está ativa");
    }

    [Fact(DisplayName = "🟢 Deve ignorar status inválido")]
    public void BuildFilter_Should_Ignore_Invalid_Status()
    {
        // Arrange - Configura filtro com status inválido
        var parameters = new SalesQueryParametersDto
        {
            Status = "invalid-status"
        };

        // Act - Executa construção do filtro
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica que status inválido é ignorado conforme regra de negócio
        filter.Should().BeNull("porque status inválido deve ser ignorado");
    }

    [Fact(DisplayName = "🟢 Deve combinar múltiplos filtros com operador AND")]
    public void BuildFilter_Should_Combine_Multiple_Filters_With_AND()
    {
        // Arrange - Configura múltiplos filtros para testar combinação com AND
        var customerId = Guid.NewGuid();
        var parameters = new SalesQueryParametersDto
        {
            SaleNumber = "SALE*",
            CustomerId = customerId,
            MinTotalAmount = 100m,
            Status = "active"
        };

        // Act - Executa construção do filtro combinado
        var filter = _filterBuilder.BuildFilter(parameters);

        // Assert - Verifica que múltiplos filtros são combinados com AND
        filter.Should().NotBeNull("porque foram fornecidos múltiplos filtros");

        var compiledFilter = filter!.Compile();

        // Teste venda que atende todos os critérios
        var testSaleMatching = new Sale
        {
            SaleNumber = "SALE-001",
            CustomerId = customerId
        };
        testSaleMatching.AddItem(new SaleItem { Quantity = 1, UnitPrice = 500m });
        testSaleMatching.CalculateTotalAmount();
        // IsCancelled já é false por padrão (ativa)
        compiledFilter(testSaleMatching).Should().BeTrue("porque atende todos os critérios");

        // Teste venda que não atende um dos critérios (valor baixo)
        var testSaleNotMatching = new Sale
        {
            SaleNumber = "SALE-001",
            CustomerId = customerId
        };
        testSaleNotMatching.AddItem(new SaleItem { Quantity = 1, UnitPrice = 50m });
        testSaleNotMatching.CalculateTotalAmount();
        // IsCancelled já é false por padrão (ativa)
        compiledFilter(testSaleNotMatching).Should().BeFalse("porque não atende o critério de valor mínimo");
    }
}
