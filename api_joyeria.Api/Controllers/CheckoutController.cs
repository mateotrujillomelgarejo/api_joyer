using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;


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

    [HttpPost]
    public async Task<IActionResult> StartCheckout([FromBody] GuestCheckoutDto dto)
    {
        var cart = await _cartService.GetCartByTokenAsync(dto.GuestToken);

        if (cart == null || cart.Items.Count == 0)
            return BadRequest("Cart is empty");

        var order = await _orderService.CreateOrderFromCartAsync(cart.Id, dto);
        return Ok(order);
    }
}