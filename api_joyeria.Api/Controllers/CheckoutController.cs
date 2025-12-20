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
        // Buscar carrito por token
        var cart = await _cartService.GetCartByTokenAsync(dto.GuestToken);

        if (cart == null || cart.Items.Count == 0)
            return BadRequest("Cart is empty");

        // Mapear GuestCheckoutDto -> CheckoutDetailsDto (solo los datos de checkout)
        var details = new CheckoutDetailsDto
        {
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            Zip = dto.Zip
        };

        var order = await _orderService.CreateOrderFromCartAsync(cart.Id, details);
        return Ok(order);
    }
}