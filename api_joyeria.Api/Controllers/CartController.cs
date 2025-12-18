using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;

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

    [HttpPost("{cartId:int}/items")]
    public async Task<IActionResult> AddItem(int cartId, [FromBody] CartItemDto dto)
    {
        var cart = await _cartService.AddItemToCartAsync(cartId, dto);
        return Ok(cart);
    }

    [HttpDelete("{cartId:int}/items/{itemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartId, int itemId)
    {
        await _cartService.RemoveItemFromCartAsync(cartId, itemId);
        return NoContent();
    }

    [HttpDelete("{cartId:int}/items")]
    public async Task<IActionResult> ClearCart(int cartId)
    {
        await _cartService.ClearCartAsync(cartId);
        return NoContent();
    }
}
