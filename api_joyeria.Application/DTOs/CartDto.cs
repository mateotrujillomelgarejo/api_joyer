namespace api_joyeria.Application.DTOs;

public class CartItemDto
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}

public class CartDto
{
    public int Id { get; set; }
    public string GuestToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}