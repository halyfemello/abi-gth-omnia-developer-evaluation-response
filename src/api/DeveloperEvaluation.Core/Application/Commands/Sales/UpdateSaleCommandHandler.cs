using MediatR;
using AutoMapper;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Domain.Events;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, SaleDto>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateSaleCommandHandler(IRepository<Sale> saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<SaleDto> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        var existingSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingSale == null)
        {
            throw new ArgumentException($"Venda com ID {request.Id} não encontrada");
        }

        if (existingSale.IsCancelled)
        {
            throw new InvalidOperationException("Não é possível atualizar uma venda cancelada");
        }

        existingSale.SaleNumber = request.SaleNumber;
        existingSale.SaleDate = request.SaleDate;
        existingSale.CustomerId = request.CustomerId;
        existingSale.CustomerName = request.CustomerName;
        existingSale.CustomerEmail = request.CustomerEmail;
        existingSale.BranchId = request.BranchId;
        existingSale.BranchName = request.BranchName;

        var itemsToRemove = existingSale.Items.ToList();
        foreach (var item in itemsToRemove)
        {
            existingSale.RemoveItem(item.Id);
        }

        foreach (var itemDto in request.Items)
        {
            var saleItem = new SaleItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                UnitPrice = itemDto.UnitPrice,
                Quantity = itemDto.Quantity
            };

            existingSale.AddItem(saleItem);
        }

        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        await _mediator.Publish(new SaleModifiedEvent(existingSale, "Venda atualizada via API"), cancellationToken);

        return _mapper.Map<SaleDto>(existingSale);
    }
}
