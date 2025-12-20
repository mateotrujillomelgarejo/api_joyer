using System;

namespace api_joyeria.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; } // FK
    public Cart? Cart { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty; // snapshot
    public decimal UnitPrice { get; set; } // snapshot
    public int Quantity { get; set; } = 1;

    public decimal Subtotal => UnitPrice * Quantity;

    public void SetQuantity(int qty)
    {
        if (qty < 1) throw new ArgumentException("Quantity must be >= 1");
        Quantity = qty;
    }
}