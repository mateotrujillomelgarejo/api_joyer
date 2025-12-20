namespace api_joyeria.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N")[..10];
    public string GuestEmail { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<OrderItem> Items { get; set; } = new();
    public OrderCustomer? Customer { get; set; }
}