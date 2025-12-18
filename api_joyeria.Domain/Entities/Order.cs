using System.Collections.Generic;

namespace api_joyeria.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;


    public int? CustomerId { get; set; }
    public OrderCustomer? Customer { get; set; }

    public int? ShippingAddressId { get; set; }
    public ShippingAddress? ShippingAddress { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}