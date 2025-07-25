using AutoMapper;
using DeveloperEvaluation.Core.Application.Commands.Carts;
using DeveloperEvaluation.Core.Domain.Repositories;
using DeveloperEvaluation.Core.Domain.Entities;
using MediatR;

namespace DeveloperEvaluation.Core.Application.Handlers.Carts;

public class CreateCartHandler : IRequestHandler<CreateCartCommand, CreateCartResponse>
{
    private readonly IRepository<Cart> _cartRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateCartHandler(
        IRepository<Cart> cartRepository,
        IMapper mapper,
        IMediator mediator)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<CreateCartResponse> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid cart data");
            }

            var cart = _mapper.Map<Cart>(request);

            var createdCart = await _cartRepository.CreateAsync(cart, cancellationToken);

            // Aqui e nós demais metodos igual o sales você pode adicionar integração com sistemas message broker, notificações, etc.
            // Igual criamos nos eventos de sales

            return _mapper.Map<CreateCartResponse>(createdCart);
        }
        catch (Exception)
        {
            // Aqui e nós demais metodos igual o sales você pode adicionar integração com sistemas message broker, notificações, etc.
            // Igual criamos nos eventos de sales

            throw;
        }
    }
}
