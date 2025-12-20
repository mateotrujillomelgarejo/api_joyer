using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs;

public class CartItemDto
{
    public int Id { get; set; }
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
    public decimal Subtotal { get; set; }           // calculado en servidor
    public List<CartItemDto> Items { get; set; } = new();
}

public class AddCartItemRequestDto
{
    [Required] public int ProductId { get; set; }
    [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
}