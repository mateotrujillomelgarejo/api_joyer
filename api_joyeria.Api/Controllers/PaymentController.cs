using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.DTOs.Payment;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PaymentController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Inicia un pago para una orden y devuelve URL/id del gateway.
        /// No confirma pagos ni cambia estados.
        /// </summary>
        [HttpPost("init")]
        public async Task<IActionResult> InitializePayment([FromBody] PaymentRequestDto dto)
        {
            if (dto == null) return BadRequest();

            var command = _mapper.Map<InitializePaymentCommand>(dto);
            var result = await _mediator.Send(command);

            var response = _mapper.Map<PaymentInitResultDto>(result);
            return Ok(response);
        }
    }
}