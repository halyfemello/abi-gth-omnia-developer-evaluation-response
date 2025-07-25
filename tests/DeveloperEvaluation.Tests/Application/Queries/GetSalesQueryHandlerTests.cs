using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Sales;
using DeveloperEvaluation.Core.Application.Services;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using FluentAssertions;
using System.Linq.Expressions;

namespace DeveloperEvaluation.Tests.Application.Queries;

/// <summary>
/// Testes unitários para GetSalesQueryHandler
/// Valida a aplicação das regras de negócio de consulta avançada
/// </summary>
public class GetSalesQueryHandlerTests
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesQueryHandler _handler;
    private readonly SaleFilterBuilder _filterBuilder;

    public GetSalesQueryHandlerTests()
    {
        _saleRepository = Substitute.For<IRepository<Sale>>();
        _mapper = Substitute.For<IMapper>();
        _filterBuilder = new SaleFilterBuilder();
        _handler = new GetSalesQueryHandler(_saleRepository, _mapper, _filterBuilder);
    }

    [Fact(DisplayName = "🟢 Deve retornar resultados paginados com sucesso")]
    public async Task Handle_Should_Return_Paged_Results_Successfully()
    {
        // Arrange - Configura cenário de consulta com paginação bem-sucedida
        var parameters = new SalesQueryParametersDto
        {
            Page = 1,
            Size = 10,
            SaleNumber = "SALE*"
        };

        var query = new GetSalesQuery(parameters);

        var salesFromRepo = new List<Sale>
        {
            new Sale { Id = Guid.NewGuid(), SaleNumber = "SALE-001" },
            new Sale { Id = Guid.NewGuid(), SaleNumber = "SALE-002" }
        };

        var salesDtos = new List<SaleDto>
        {
            new SaleDto { Id = salesFromRepo[0].Id, SaleNumber = "SALE-001" },
            new SaleDto { Id = salesFromRepo[1].Id, SaleNumber = "SALE-002" }
        };

        // Configura mock do repositório para retornar dados paginados
        _saleRepository.GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((salesFromRepo, 2L));

        // Configura mock do mapper
        _mapper.Map<IEnumerable<SaleDto>>(salesFromRepo).Returns(salesDtos);

        // Act - Executa a consulta
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Verifica se retorna resultado paginado conforme regras de negócio
        result.Should().NotBeNull("porque a consulta deve retornar resultado");
        result.Data.Should().HaveCount(2, "porque foram retornados 2 itens do repositório");
        result.CurrentPage.Should().Be(1, "porque foi solicitada a página 1");
        result.PageSize.Should().Be(10, "porque foi solicitado tamanho 10");
        result.TotalItems.Should().Be(2, "porque o repositório retornou total de 2 itens");
        result.TotalPages.Should().Be(1, "porque com 2 itens e página de tamanho 10, há apenas 1 página");
        result.HasPreviousPage.Should().BeFalse("porque está na primeira página");
        result.HasNextPage.Should().BeFalse("porque só há uma página");

        // Verifica se o repositório foi chamado com os parâmetros corretos
        await _saleRepository.Received(1).GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            parameters.Order,
            parameters.Page,
            parameters.Size,
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "🔴 Deve lançar exceção quando intervalo de datas for inválido")]
    public async Task Handle_Should_Throw_Exception_When_Invalid_Date_Range()
    {
        // Arrange - Configura cenário com range de data inválido
        var parameters = new SalesQueryParametersDto
        {
            MinSaleDate = new DateTime(2024, 12, 31),
            MaxSaleDate = new DateTime(2024, 1, 1) // Data máxima anterior à mínima
        };

        var query = new GetSalesQuery(parameters);

        // Act & Assert - Verifica se aplica regra de negócio de validação de range de data
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Be("Data mínima deve ser anterior ou igual à data máxima",
            "porque a regra de negócio exige consistência no range de datas");

        // Verifica que o repositório não foi chamado quando há erro de validação
        await _saleRepository.DidNotReceive().GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "🔴 Deve lançar exceção quando intervalo de valores for inválido")]
    public async Task Handle_Should_Throw_Exception_When_Invalid_Amount_Range()
    {
        // Arrange - Configura cenário com range de valor inválido
        var parameters = new SalesQueryParametersDto
        {
            MinTotalAmount = 1000m,
            MaxTotalAmount = 100m // Valor máximo menor que o mínimo
        };

        var query = new GetSalesQuery(parameters);

        // Act & Assert - Verifica se aplica regra de negócio de validação de range de valor
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Be("Valor mínimo deve ser menor ou igual ao valor máximo",
            "porque a regra de negócio exige consistência no range de valores");

        // Verifica que o repositório não foi chamado quando há erro de validação
        await _saleRepository.DidNotReceive().GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "🟢 Deve lidar com resultados vazios")]
    public async Task Handle_Should_Handle_Empty_Results()
    {
        // Arrange - Configura cenário onde não há resultados
        var parameters = new SalesQueryParametersDto
        {
            Page = 1,
            Size = 10,
            SaleNumber = "NON-EXISTENT"
        };

        var query = new GetSalesQuery(parameters);

        // Configura mock para retornar lista vazia
        _saleRepository.GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 0L));

        _mapper.Map<IEnumerable<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(new List<SaleDto>());

        // Act - Executa a consulta
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Verifica se trata corretamente resultado vazio conforme regras de negócio
        result.Should().NotBeNull("porque sempre deve retornar um resultado, mesmo que vazio");
        result.Data.Should().BeEmpty("porque não há vendas que atendam aos critérios");
        result.TotalItems.Should().Be(0, "porque não há itens");
        result.TotalPages.Should().Be(0, "porque sem itens não há páginas");
        result.HasPreviousPage.Should().BeFalse("porque não há páginas");
        result.HasNextPage.Should().BeFalse("porque não há páginas");
    }

    [Fact(DisplayName = "🟢 Deve passar parâmetro de ordenação correto para o repositório")]
    public async Task Handle_Should_Pass_Correct_Order_Parameter_To_Repository()
    {
        // Arrange - Configura cenário para testar passagem de parâmetro de ordenação
        var parameters = new SalesQueryParametersDto
        {
            Page = 1,
            Size = 10,
            Order = "saleDate desc, customerName asc"
        };

        var query = new GetSalesQuery(parameters);

        _saleRepository.GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 0L));

        _mapper.Map<IEnumerable<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(new List<SaleDto>());

        // Act - Executa a consulta
        await _handler.Handle(query, CancellationToken.None);

        // Assert - Verifica se o parâmetro de ordenação é passado corretamente
        await _saleRepository.Received(1).GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            "saleDate desc, customerName asc", // Verifica ordenação específica
            1,
            10,
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "🟢 Deve aplicar validação de tamanho da página")]
    public async Task Handle_Should_Apply_Page_Size_Validation()
    {
        // Arrange - Configura cenário para testar validação de tamanho de página
        var parameters = new SalesQueryParametersDto
        {
            Page = 2,
            Size = 50 // Tamanho válido dentro do limite
        };

        var query = new GetSalesQuery(parameters);

        _saleRepository.GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns((new List<Sale>(), 100L));

        _mapper.Map<IEnumerable<SaleDto>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(new List<SaleDto>());

        // Act - Executa a consulta
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - Verifica se os parâmetros de paginação são aplicados corretamente
        result.CurrentPage.Should().Be(2, "porque foi solicitada a página 2");
        result.PageSize.Should().Be(50, "porque foi solicitado tamanho 50");
        result.TotalPages.Should().Be(2, "porque com 100 itens e página de tamanho 50, há 2 páginas");
        result.HasPreviousPage.Should().BeTrue("porque está na página 2");
        result.HasNextPage.Should().BeFalse("porque está na última página");

        // Verifica chamada ao repositório com parâmetros corretos
        await _saleRepository.Received(1).GetPagedAsync(
            Arg.Any<Expression<Func<Sale, bool>>>(),
            Arg.Any<string>(),
            2, // Página correta
            50, // Tamanho correto
            Arg.Any<CancellationToken>());
    }
}
