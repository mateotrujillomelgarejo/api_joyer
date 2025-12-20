using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs
{
    public class CheckoutRequestDto
    {
        [Required] public string GuestToken { get; set; } = string.Empty;
        [Required] public string GuestName { get; set; } = string.Empty;
        [Required, EmailAddress] public string GuestEmail { get; set; } = string.Empty;

        [Required] public string Street { get; set; } = string.Empty;
        [Required] public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Country { get; set; } = "US";
        public string? Phone { get; set; }

        // Optional: shipping method, idempotency key
        public string? ShippingMethodId { get; set; }
        public string? IdempotencyKey { get; set; } // recommended for avoiding duplicate order creation
    }
}
