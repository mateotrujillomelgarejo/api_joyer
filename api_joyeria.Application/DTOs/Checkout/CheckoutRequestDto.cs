using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs.Checkout
{
    public class CheckoutRequestDto
    {
        public string CartId { get; set; }
        public string Email { get; set; }
        public AddressDto ShippingAddress { get; set; }
    }
}
