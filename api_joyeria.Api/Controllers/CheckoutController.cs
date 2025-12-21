using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.DTOs.Checkout;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CheckoutController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Crea una orden guest en estado Pending.
        /// - No contiene lógica de negocio.
        /// - Mapea DTO -> Command y delega a Application (MediatR).
        /// </summary>
        [HttpPost("guest")]
        public async Task<IActionResult> CreateGuestOrder([FromBody] GuestCheckoutDto dto)
        {
            if (dto == null) return BadRequest();

            var command = _mapper.Map<CreateGuestOrderCommand>(dto);
            var result = await _mediator.Send(command);

            var response = _mapper.Map<CheckoutResultDto>(result);

            return CreatedAtAction(nameof(GetOrderStatus), new { id = response.OrderId }, response);
        }

        // Placeholder para Location header; la implementación real debe venir de Application/Repository.
        [HttpGet("{id}/status")]
        public IActionResult GetOrderStatus(string id)
            => Ok(new { orderId = id, status = "Pending" });
    }
}