using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces.Services;
using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            if (dto == null) return BadRequest();
            await _cartService.AddItemAsync(dto.CartId, dto.ProductId, dto.Quantity);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId)) return BadRequest();
            var cart = await _cartService.GetCartAsync(cartId);
            if (cart == null) return NotFound();
            // Map domain Cart -> API DTO via AutoMapper or build lightweight DTO here.
            // Assuming AutoMapper is wired, controllers should return DTOs. For brevity, return domain object.
            return Ok(cart);
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItem(string itemId, [FromBody] UpdateCartItemDto dto)
        {
            if (dto == null) return BadRequest();
            await _cartService.UpdateItemQuantityAsync(dto.CartId, itemId, dto.Quantity);
            return NoContent();
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(string itemId, [FromQuery] string cartId)
        {
            if (string.IsNullOrWhiteSpace(cartId)) return BadRequest();
            await _cartService.RemoveItemAsync(cartId, itemId);
            return NoContent();
        }
    }
}