using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Queries.Sales;

public class GetSaleByIdQuery : IRequest<SaleDto?>
{
    public Guid Id { get; set; }

    public GetSaleByIdQuery(Guid id)
    {
        Id = id;
    }
}

public class GetSalesQuery : IRequest<PagedResultDto<SaleDto>>
{
    public SalesQueryParametersDto Parameters { get; set; }

    public GetSalesQuery(SalesQueryParametersDto parameters)
    {
        Parameters = parameters;
    }
}

public class GetAllSalesQuery : IRequest<IEnumerable<SaleDto>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    public GetAllSalesQuery(int page = 1, int size = 10)
    {
        Page = page;
        Size = size;
    }
}
