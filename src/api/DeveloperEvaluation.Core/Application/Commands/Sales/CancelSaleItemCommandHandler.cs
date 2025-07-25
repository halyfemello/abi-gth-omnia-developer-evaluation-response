using MediatR;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Events;
using DeveloperEvaluation.Core.Domain.Entities;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class CancelSaleItemCommandHandler : IRequestHandler<CancelSaleItemCommand, bool>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMediator _mediator;

    public CancelSaleItemCommandHandler(IRepository<Sale> saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new ArgumentException($"Venda com ID {request.SaleId} não encontrada");
        }

        if (sale.IsCancelled)
        {
            throw new InvalidOperationException("Não é possível cancelar itens de uma venda já cancelada");
        }

        var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);
        if (item == null)
        {
            throw new ArgumentException($"Item com ID {request.ItemId} não encontrado na venda {request.SaleId}");
        }

        item.Cancel();

        sale.CalculateTotalAmount();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mediator.Publish(new ItemCancelledEvent(sale, item, "Item cancelado via API"), cancellationToken);

        return true;
    }
}
