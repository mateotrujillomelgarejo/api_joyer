using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpDelete("{cartId}/items/{itemId}")]
    public async Task<IActionResult> RemoveItem(int cartId, int itemId)
    {
        await _cartService.RemoveItemFromCartAsync(cartId, itemId);
        return NoContent();
    }


    [HttpPost("{cartId}/items")]
    public async Task<IActionResult> AddItem(int cartId, CartItemDto dto)
    {
        var cart = await _cartService.AddItemToCartAsync(cartId, dto);
        return Ok(cart);
    }


    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCarts()
    {
        var carts = await _cartService.GetActiveCartsAsync();
        return Ok(carts);
    }

    [HttpGet("guest/{guestToken}")]
    public async Task<IActionResult> GetCartByGuestToken(string guestToken)
    {
        var cart = await _cartService.GetCartByTokenAsync(guestToken);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpDelete("{cartId}/clear")]
    public async Task<IActionResult> ClearCart(int cartId)
    {
        await _cartService.ClearCartAsync(cartId);
        return NoContent();
    }

    [HttpGet("{cartId}")]
    public async Task<IActionResult> GetById(int cartId)
    {
        var cart = await _cartService.GetCartByIdAsync(cartId);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

}