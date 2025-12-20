using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs.Checkout;

public class GuestCheckoutDto
{
    [Required]
    public string GuestToken { get; set; } = string.Empty;

    [Required] public string GuestName { get; set; } = string.Empty;
    [Required, EmailAddress] public string GuestEmail { get; set; } = string.Empty;

    [Required] public string Street { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}

public class CheckoutStartRequestDto
{
    public string GuestToken { get; set; } = string.Empty;

    public string? GuestEmail { get; set; }
    public ShippingAddressDto? ShippingAddress { get; set; }

    public string? IdempotencyKey { get; set; }
}

public class ShippingAddressDto
{
    [Required] public string Street { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = "US";
    public string? Phone { get; set; }
}