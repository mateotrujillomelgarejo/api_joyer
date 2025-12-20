using System;

namespace api_joyeria.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // snapshot
    public string SKU { get; set; } = string.Empty; // snapshot
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // historical price

    public decimal Subtotal => UnitPrice * Quantity;
}