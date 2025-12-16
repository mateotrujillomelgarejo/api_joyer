using Microsoft.AspNetCore.Mvc;
using api_joyeria.Application.Interfaces;
using api_joyeria.Application.DTOs;

namespace api_joyeria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IOrderService _orderService;

    public CheckoutController(IOrderService orderService) => _orderService = orderService;

    [HttpPost("guest")]
    public async Task<IActionResult> GuestCheckout([FromBody] GuestCheckoutDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var order = await _orderService.GuestCheckoutAsync(dto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (KeyNotFoundException knf)
        {
            return NotFound(new { message = knf.Message });
        }
        catch (InvalidOperationException ioe)
        {
            return BadRequest(new { message = ioe.Message });
        }
        catch (ArgumentException ae)
        {
            return BadRequest(new { message = ae.Message });
        }
    }

    [HttpGet("{id:int}", Name = "GetOrder")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
}