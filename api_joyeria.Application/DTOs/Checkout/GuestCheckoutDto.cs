using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs.Checkout;

public class GuestCheckoutDto
{
    public string CartId { get; set; }
    public string Email { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public List<CreateGuestOrderItemDto> Items { get; set; }
}