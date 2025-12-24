namespace api_joyeria.Application.DTOs;

public class ProductoDto
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
}