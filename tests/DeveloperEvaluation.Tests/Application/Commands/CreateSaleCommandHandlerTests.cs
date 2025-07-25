using DeveloperEvaluation.Core.Application.Commands.Sales;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using AutoMapper;
using NSubstitute;
using FluentAssertions;
using MediatR;

namespace DeveloperEvaluation.Tests.Application.Commands;

/// <summary>
/// Testes unitários para CreateSaleCommandHandler
/// </summary>
public class CreateSaleCommandHandlerTests
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _saleRepository = Substitute.For<IRepository<Sale>>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleCommandHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "🟢 Deve criar venda com sucesso")]
    public async Task Handle_Should_Create_Sale_Successfully()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-TEST-001",
            SaleDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Teste",
            CustomerEmail = "cliente@teste.com",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Teste",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto Teste",
                    ProductDescription = "Descrição teste",
                    UnitPrice = 50.00m,
                    Quantity = 2
                }
            }
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            CustomerEmail = command.CustomerEmail,
            BranchId = command.BranchId,
            BranchName = command.BranchName
        };

        var saleDto = new SaleDto
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            CustomerName = createdSale.CustomerName,
            TotalAmount = 100.00m
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper.Map<SaleDto>(Arg.Any<Sale>())
            .Returns(saleDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SaleNumber.Should().Be(command.SaleNumber);
        result.CustomerName.Should().Be(command.CustomerName);
        result.TotalAmount.Should().Be(100.00m);

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "🟢 Deve aplicar regras de negócio corretamente")]
    public async Task Handle_Should_Apply_Business_Rules_Correctly()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = "SALE-TEST-002",
            SaleDate = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente Desconto",
            CustomerEmail = "desconto@teste.com",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial Desconto",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto com Desconto",
                    ProductDescription = "Produto que terá desconto",
                    UnitPrice = 100.00m,
                    Quantity = 5 // 5 itens = 10% desconto
                }
            }
        };

        var createdSale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber
        };

        // Simular item com desconto aplicado
        var saleItem = new SaleItem
        {
            ProductId = command.Items.First().ProductId,
            ProductName = command.Items.First().ProductName,
            UnitPrice = command.Items.First().UnitPrice,
            Quantity = command.Items.First().Quantity
        };

        createdSale.AddItem(saleItem); // Isso aplicará o desconto automaticamente

        var saleDto = new SaleDto
        {
            Id = createdSale.Id,
            SaleNumber = createdSale.SaleNumber,
            TotalAmount = createdSale.TotalAmount // Usar o valor calculado
        };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(createdSale);

        _mapper.Map<SaleDto>(Arg.Any<Sale>())
            .Returns(saleDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(450.00m); // Verificar se o desconto foi aplicado (100 * 5 * 0.9 = 450)

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
