using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Domain.Entities
{
    public sealed class Producto
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public Money Price { get; private set; }
        public int Stock { get; private set; }

        private Producto() { }

        public Producto(string id, string nombre, string descripcion, Money price, int stock)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new DomainException("Product id required");
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Price = price ?? throw new DomainException("Price required");
            Stock = stock;
        }

        public bool HasSufficientStock(int required) => Stock >= required;

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be positive");
            if (Stock < quantity) throw new DomainException($"Not enough stock for product {Id}");
            Stock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0) throw new DomainException("Quantity must be positive");
            Stock += quantity;
        }
    }
}