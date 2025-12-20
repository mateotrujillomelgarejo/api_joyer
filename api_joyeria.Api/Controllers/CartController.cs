using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces.Services;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCart()
    {
        var cart = await _cartService.CreateCartAsync();
        return Ok(cart);
    }

    [HttpGet("{cartId:int}")]
    public async Task<IActionResult> GetCartById(int cartId)
    {
        var cart = await _cartService.GetCartByIdAsync(cartId);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpGet("guest/{guestToken}")]
    public async Task<IActionResult> GetByToken(string guestToken)
    {
        var cart = await _cartService.GetCartByTokenAsync(guestToken);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPost("{guestToken}/items")]
    public async Task<IActionResult> AddItem(string guestToken, [FromBody] AddCartItemRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var cart = await _cartService.AddItemToCartAsync(guestToken, dto);
        return Ok(cart);
    }

    [HttpDelete("{guestToken}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(string guestToken, int itemId)
    {
        await _cartService.RemoveItemFromCartAsync(guestToken, itemId);
        return NoContent();
    }

    [HttpDelete("{guestToken}/items")]
    public async Task<IActionResult> ClearCart(string guestToken)
    {
        await _cartService.ClearCartAsync(guestToken);
        return NoContent();
    }
}