using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Application.Interfaces.Services;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public CheckoutController(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartCheckout([FromBody] CheckoutStartRequestDto dto)
    {
        // dto includes GuestToken, optional Shipping and IdempotencyKey if you want
        // Validate cart and mark status = CHECKOUT_STARTED, set CheckoutStartedAt
        await _cartService.StartCheckoutAsync(dto.GuestToken, dto); // new method
        return Ok(new { message = "Checkout started" });
    }

    [HttpPost("/api/orders/guest")]
    public async Task<IActionResult> CreateOrder([FromBody] CheckoutRequestDto dto)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].ToString();
        if (string.IsNullOrWhiteSpace(idempotencyKey)) return BadRequest("Missing Idempotency-Key header");

        var order = await _orderService.CreateOrderFromCartAsync(dto.GuestToken, dto, idempotencyKey);
        return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
    }

    [HttpGet("/api/orders/{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var order = await _orderService.GetByIdAsync(orderId);

        if (order == null)
            return NotFound();

        return Ok(order);
    }

}