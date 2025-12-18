namespace api_joyeria.Domain.Entities;

public class ShippingAddress
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}