using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Auth;
using DeveloperEvaluation.Core.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperEvaluation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
    {
        if (!ModelState.IsValid)
        {
            throw new ArgumentException("Dados de entrada inválidos");
        }

        var command = _mapper.Map<LoginCommand>(loginRequest);

        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            if (response.Message.Contains("inválidos") || response.Message.Contains("inativo"))
            {
                throw new UnauthorizedAccessException(response.Message);
            }
            throw new ArgumentException(response.Message);
        }

        var responseDto = _mapper.Map<LoginResponseDto>(response);

        return Ok(responseDto);
    }
}
