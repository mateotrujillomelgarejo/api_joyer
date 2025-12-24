using api_joyeria.Api.Dtos;
using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;
using api_joyeria.Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            _cartService = cartService;
            _mapper = mapper;
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto,CancellationToken cancellationToken)
        {
            if (dto == null) return BadRequest();

            var cartId = await _cartService.AddItemAsync(
                dto.CartId,
                dto.ProductId,
                dto.Quantity,
                cancellationToken
            );

            return Ok(new { cartId });
        }


        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] string cartId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cartId)) return BadRequest();
            var cart = await _cartService.GetCartAsync(cartId, cancellationToken);
            if (cart == null) return NotFound();

            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateItem(string productId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) return BadRequest();
            await _cartService.UpdateItemQuantityAsync(dto.CartId, productId, dto.Quantity, cancellationToken);
            return NoContent();
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(string productId, [FromQuery] string cartId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(cartId)) return BadRequest();
            await _cartService.RemoveItemAsync(cartId, productId, cancellationToken);
            return NoContent();
        }
    }
}