using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Users;
using DeveloperEvaluation.Core.Application.DTOs;
using DeveloperEvaluation.Core.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperEvaluation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<UserDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PagedResultDto<UserDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? order = null,
        [FromQuery] string? username = null,
        [FromQuery] string? email = null,
        [FromQuery] string? status = null)
    {

        var response = await _mediator.Send(new GetUsersQuery
        {
            Page = page,
            Size = size,
            Order = order,
            Username = username,
            Email = email,
            Status = status
        });

        if (!response.Success)
        {
            throw new ArgumentException(response.Message);
        }

        return Ok(response.Data);
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreateUserResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<CreateUserResponse>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<CreateUserCommand>(createUserDto);
        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetUserByIdResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<GetUserByIdResponse>> GetUserById(Guid id)
    {
        var response = await _mediator.Send(new GetUserByIdQuery(id));

        if (!response.Success)
        {
            if (response.Message.Contains("não encontrado"))
            {
                return NotFound(response);
            }
            throw new ArgumentException(response.Message);
        }

        return Ok(response.User);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateUserResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<UpdateUserResponse>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = _mapper.Map<UpdateUserCommand>(updateUserDto);
        command.Id = id;

        var response = await _mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DeleteUserResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<DeleteUserResponse>> DeleteUser(Guid id)
    {
        var response = await _mediator.Send(new DeleteUserCommand(id));

        return Ok(response);
    }
}
