using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Products;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperEvaluation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetProducts(
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string? _order = null)
    {
        var query = new GetProductsQuery
        {
            Page = _page,
            Size = _size,
            Order = _order
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            throw new ArgumentException(response.Message);
        }

        var result = new
        {
            data = response.Data.Data,
            totalCount = response.Data.TotalItems,
            page = response.Data.CurrentPage,
            size = query.Size,
            totalPages = response.Data.TotalPages
        };

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<CreateProductCommand>(createProductDto);
        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProductById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var response = await _mediator.Send(query);

        if (response == null || !response.Success)
        {
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
        }

        return Ok(response.Product);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<UpdateProductCommand>(updateProductDto);
        command.Id = id;

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var command = new DeleteProductCommand(id);
        await _mediator.Send(command);

        return Ok(new { message = "Produto excluído com sucesso" });
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<string[]>> GetCategories()
    {
        var query = new GetAllProductsQuery();
        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            throw new ArgumentException(response.Message);
        }

        var categories = response.Products
            .Select(p => p.Category)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToArray();

        return Ok(categories);
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetProductsByCategory(
        string category,
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string? _order = null)
    {
        var query = new GetProductsQuery
        {
            Page = _page,
            Size = _size,
            Order = _order,
            Category = category
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            throw new ArgumentException(response.Message);
        }

        var result = new
        {
            data = response.Data.Data,
            totalCount = response.Data.TotalItems,
            page = response.Data.CurrentPage,
            size = response.Data.Data?.Count() ?? 0,
            totalPages = response.Data.TotalPages
        };

        return Ok(result);
    }
}