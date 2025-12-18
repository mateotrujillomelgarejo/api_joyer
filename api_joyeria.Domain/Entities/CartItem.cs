namespace api_joyeria.Domain.Entities;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; } // Foreign key to the Cart
    public Cart? Cart { get; set; } // Navigation property

    public int ProductId { get; set; } // Reference to the product
    public string ProductName { get; set; } = string.Empty; // Name of the product
    public decimal UnitPrice { get; set; } // Price per unit of the product
    public int Quantity { get; set; } // Quantity of the product in the cart
    public decimal Subtotal => UnitPrice * Quantity; // Calculate subtotal
}