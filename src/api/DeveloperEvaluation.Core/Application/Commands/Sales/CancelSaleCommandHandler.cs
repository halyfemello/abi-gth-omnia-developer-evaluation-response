using MediatR;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Events;
using DeveloperEvaluation.Core.Domain.Entities;

namespace DeveloperEvaluation.Core.Application.Commands.Sales;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, bool>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMediator _mediator;

    public CancelSaleCommandHandler(IRepository<Sale> saleRepository, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
        {
            throw new ArgumentException($"Venda com ID {request.SaleId} não encontrada");
        }

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleCancelledEvent(sale, "Venda cancelada via API"), cancellationToken);

        return true;
    }
}
