namespace api_joyeria.Domain.Entities;

public class OrderCustomer
{
    public int Id { get; set; } // ID único.
    public int OrderId { get; set; } // Relacionado con la orden.
    public string FullName { get; set; } // Nombre y apellido del cliente.
    public string Phone { get; set; } // Teléfono contacto o envío.

    public Order Order { get; set; } // Relación inversa.
}