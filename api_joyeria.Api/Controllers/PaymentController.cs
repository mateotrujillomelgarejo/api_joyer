using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Application.Interfaces.Services;

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
    public async Task<IActionResult> Pay(int orderId, [FromBody] PaymentRequestDto dto)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].ToString();
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return BadRequest("Missing Idempotency-Key header");

        var result = await _paymentService.ProcessPaymentAsync(orderId, dto.Method, idempotencyKey);
        return Ok(result);
    }

}