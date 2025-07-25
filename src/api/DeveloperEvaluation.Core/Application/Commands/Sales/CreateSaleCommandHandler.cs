using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Events;
using MediatR;
using AutoMapper;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateSaleCommandHandler(
        IRepository<Sale> saleRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = new Sale
        {
            SaleNumber = request.SaleNumber,
            SaleDate = request.SaleDate,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            BranchId = request.BranchId,
            BranchName = request.BranchName
        };

        foreach (var itemDto in request.Items)
        {
            var item = new SaleItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                ProductDescription = itemDto.ProductDescription,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                SaleId = sale.Id
            };

            sale.AddItem(item);
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleCreatedEvent(createdSale), cancellationToken);

        return _mapper.Map<SaleDto>(createdSale);
    }
}
