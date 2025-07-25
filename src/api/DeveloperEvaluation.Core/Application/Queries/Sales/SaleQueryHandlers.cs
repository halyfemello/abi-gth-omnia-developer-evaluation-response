using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using DeveloperEvaluation.Core.Application.Services;
using MediatR;
using AutoMapper;

namespace DeveloperEvaluation.Core.Application.Queries.Sales;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleByIdQueryHandler(IRepository<Sale> saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        return sale == null ? null : _mapper.Map<SaleDto>(sale);
    }
}

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, PagedResultDto<SaleDto>>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;
    private readonly SaleFilterBuilder _filterBuilder;

    public GetSalesQueryHandler(IRepository<Sale> saleRepository, IMapper mapper, SaleFilterBuilder filterBuilder)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _filterBuilder = filterBuilder;
    }

    public async Task<PagedResultDto<SaleDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        if (!request.Parameters.IsValidDateRange())
        {
            throw new ArgumentException("Data mínima deve ser anterior ou igual à data máxima");
        }

        if (!request.Parameters.IsValidAmountRange())
        {
            throw new ArgumentException("Valor mínimo deve ser menor ou igual ao valor máximo");
        }

        var filter = _filterBuilder.BuildFilter(request.Parameters);

        var (sales, totalCount) = await _saleRepository.GetPagedAsync(
            filter: filter,
            orderBy: request.Parameters.Order,
            page: request.Parameters.Page,
            size: request.Parameters.Size,
            cancellationToken: cancellationToken);

        var salesDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);

        return new PagedResultDto<SaleDto>(
            data: salesDtos,
            currentPage: request.Parameters.Page,
            pageSize: request.Parameters.Size,
            totalItems: totalCount);
    }
}

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, IEnumerable<SaleDto>>
{
    private readonly IRepository<Sale> _saleRepository;
    private readonly IMapper _mapper;

    public GetAllSalesQueryHandler(IRepository<Sale> saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SaleDto>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(request.Page, request.Size, cancellationToken);

        return _mapper.Map<IEnumerable<SaleDto>>(sales);
    }
}
