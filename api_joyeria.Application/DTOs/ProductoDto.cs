namespace api_joyeria.Application.DTOs;

public class ProductoDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal UnitPrice { get; set; } // renamed from Precio
    public int Stock { get; set; }
}