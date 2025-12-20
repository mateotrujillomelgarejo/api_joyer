
using api_joyeria.Domain.Enums;

namespace api_joyeria.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int? CartId { get; set; } // optional reference to original cart
    public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N")[..10];

    public string GuestEmail { get; set; } = string.Empty;

    // Value objects embedded
    public OrderCustomer? Customer { get; set; }
    public ShippingAddress? Shipping { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public decimal ShippingCost { get; set; } = 0m;
    public decimal Tax { get; set; } = 0m;

    public decimal Total => Items.Sum(i => i.Subtotal) + ShippingCost + Tax;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Payment> Payments { get; set; } = new();

    // Domain validations
    public void Validate()
    {
        if (Items == null || Items.Count == 0) throw new InvalidOperationException("Order has no items.");
        if (Total <= 0) throw new InvalidOperationException("Order total must be positive.");
    }

    public void MarkPaid()
    {
        if (Status == OrderStatus.Paid) return;
        if (Status == OrderStatus.Cancelled) throw new InvalidOperationException("Cancelled orders cannot be marked paid.");
        Status = OrderStatus.Paid;
    }

    public void MarkPaymentFailed()
    {
        if (Status == OrderStatus.Paid) throw new InvalidOperationException("Paid order cannot be marked as payment failed.");
        Status = OrderStatus.PaymentFailed;
    }
}