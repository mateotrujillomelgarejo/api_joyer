using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs;

public class GuestCheckoutItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}

public class GuestCheckoutDto
{
    [Required] public string GuestName { get; set; } = string.Empty;
    [Required, EmailAddress] public string GuestEmail { get; set; } = string.Empty;

    [Required] public string Street { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;

    [Required] public List<GuestCheckoutItemDto> Items { get; set; } = new();
}