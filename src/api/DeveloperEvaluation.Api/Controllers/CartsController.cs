using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Carts;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Carts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperEvaluation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CartsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CartsPagedResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CartsPagedResponseDto>> GetCarts(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? order = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateTime? minDate = null,
        [FromQuery] DateTime? maxDate = null)
    {
        var query = new GetCartsQuery
        {
            Page = page,
            Size = size,
            Order = order,
            UserId = userId,
            MinDate = minDate,
            MaxDate = maxDate
        };

        var response = await _mediator.Send(query);

        if (!response.Success)
        {
            throw new ArgumentException(response.Message);
        }

        return Ok(response.Data);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateCartResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CreateCartResponse>> CreateCart([FromBody] CreateCartDto createCartDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<CreateCartCommand>(createCartDto);
        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetCartById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCartByIdResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<GetCartByIdResponse>> GetCartById(Guid id)
    {
        var query = new GetCartByIdQuery { Id = id };
        var response = await _mediator.Send(query);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateCartResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<UpdateCartResponse>> UpdateCart(Guid id, [FromBody] UpdateCartDto updateCartDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<UpdateCartCommand>(updateCartDto);
        command.Id = id;

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DeleteCartResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<DeleteCartResponse>> DeleteCart(Guid id)
    {
        var command = new DeleteCartCommand { Id = id };
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}
