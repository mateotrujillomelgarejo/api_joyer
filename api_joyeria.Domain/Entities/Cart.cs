using System;
using System.Collections.Generic;
using System.Linq;

namespace api_joyeria.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public string GuestToken { get; set; } = string.Empty; // public identifier
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiredAt { get; set; }
    public bool IsConsumed { get; set; } = false; // Marked when converted to Order

    // Checkout state & temporary guest info (used during checkout flow)
    public bool IsInCheckout { get; set; } = false;
    public DateTime? CheckoutStartedAt { get; set; }
    public string? GuestEmail { get; set; }
    public ShippingAddress? ShippingAddress { get; set; }

    public List<CartItem> Items { get; set; } = new();

    // Domain behaviors / invariants
    public void EnsureNotExpired()
    {
        if (IsConsumed) throw new InvalidOperationException("Cart already consumed.");
        if (ExpiredAt != null && ExpiredAt <= DateTime.UtcNow) throw new InvalidOperationException("Cart expired.");
    }

    public void AddOrUpdateItem(int productId, string productName, decimal unitPrice, int quantity)
    {
        EnsureNotExpired();
        if (quantity < 1) throw new ArgumentException("Quantity must be >= 1");

        var existing = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity += quantity;
            existing.UnitPrice = unitPrice;
            existing.ProductName = productName;
        }
        else
        {
            Items.Add(new CartItem
            {
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity
            });
        }
    }

    public void RemoveItem(int itemId)
    {
        EnsureNotExpired();
        Items.RemoveAll(i => i.Id == itemId);
    }

    public decimal Subtotal => Items.Sum(i => i.Subtotal);

    public decimal Total(decimal shipping = 0m, decimal tax = 0m) => Subtotal + shipping + tax;
}