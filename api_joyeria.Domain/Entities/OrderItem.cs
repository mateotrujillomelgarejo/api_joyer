namespace api_joyeria.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; } // ID único.
    public Guid OrderId { get; set; } // Relacionado con el pedido.
    public Guid ProductId { get; set; } // Relacionado con el producto comprado.
    public int Quantity { get; set; } // Cantidad de productos.
    public decimal Price { get; set; } // Precio unitario cuando se compró.
}