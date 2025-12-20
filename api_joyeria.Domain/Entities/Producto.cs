using System;

namespace api_joyeria.Domain.Entities;

public class Producto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public byte[]? RowVersion { get; set; }
}