using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("{orderId:int}")]
    public async Task<IActionResult> Pay(int orderId, [FromBody] PaymentDto dto)
    {
        await _paymentService.ProcessPaymentAsync(orderId, dto.Method);
        return Ok(new { message = "Pago realizado con éxito" });
    }
}