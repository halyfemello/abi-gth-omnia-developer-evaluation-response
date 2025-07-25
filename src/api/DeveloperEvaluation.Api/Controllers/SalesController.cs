using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Sales;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperEvaluation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<SaleDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PagedResultDto<SaleDto>>> GetSales(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? order = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? branchName = null,
        [FromQuery] DateTime? minSaleDate = null,
        [FromQuery] DateTime? maxSaleDate = null,
        [FromQuery] string? saleNumber = null,
        [FromQuery] string? customerName = null,
        [FromQuery] string? status = null)
    {
        var parameters = new SalesQueryParametersDto
        {
            Page = page,
            Size = size,
            Order = order,
            CustomerId = customerId,
            BranchName = branchName,
            MinSaleDate = minSaleDate,
            MaxSaleDate = maxSaleDate,
            SaleNumber = saleNumber,
            CustomerName = customerName,
            Status = status
        };

        if (!parameters.IsValidDateRange())
        {
            throw new ArgumentException("Data mínima deve ser anterior ou igual à data máxima");
        }

        var query = new GetSalesQuery(parameters);
        var response = await _mediator.Send(query);

        var result = new
        {
            data = response.Data,
            totalCount = response.TotalItems,
            page = response.CurrentPage,
            size = parameters.Size, // Usar o tamanho da página solicitado
            totalPages = response.TotalPages
        };

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SaleDto>> CreateSale([FromBody] CreateSaleDto createSaleDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        foreach (var item in createSaleDto.Items)
        {
            if (item.Quantity > 20)
            {
                throw new ArgumentException($"Não é possível vender mais de 20 itens idênticos. Produto: {item.ProductName}");
            }
        }

        var command = _mapper.Map<CreateSaleCommand>(createSaleDto);
        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetSaleById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SaleDto>> GetSaleById(Guid id)
    {
        var query = new GetSaleByIdQuery(id);
        var response = await _mediator.Send(query);

        if (response == null)
        {
            throw new KeyNotFoundException($"Venda com ID {id} não encontrada");
        }

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SaleDto>> UpdateSale(Guid id, [FromBody] UpdateSaleDto updateSaleDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<UpdateSaleCommand>(updateSaleDto);
        command.Id = id;

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(SaleDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SaleDto>> CancelSale(Guid id)
    {
        var command = new CancelSaleCommand(id);
        var response = await _mediator.Send(command);

        if (!response)
        {
            throw new KeyNotFoundException($"Venda com ID {id} não encontrada");
        }

        var getSaleQuery = new GetSaleByIdQuery(id);
        var saleDto = await _mediator.Send(getSaleQuery);

        if (saleDto == null)
        {
            throw new KeyNotFoundException($"Venda com ID {id} não encontrada");
        }

        return Ok(saleDto);
    }

    [HttpDelete("{saleId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(SaleDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SaleDto>> CancelSaleItem(Guid saleId, Guid itemId)
    {
        var command = new CancelSaleItemCommand(saleId, itemId);
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}