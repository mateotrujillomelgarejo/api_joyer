using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Domain.Entities
{
    public sealed class OrderItem
    {
        public string Id { get; private set; }
        public string ProductId { get; private set; }
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
        public Money Total => UnitPrice.Multiply(Quantity);

        private OrderItem() { }

        // ctor mínimo
        public OrderItem(string productId, int quantity, Money unitPrice)
        {
            if (string.IsNullOrWhiteSpace(productId)) throw new DomainException("ProductId is required");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero");
            UnitPrice = unitPrice ?? throw new DomainException("UnitPrice is required");

            Id = Guid.NewGuid().ToString("N");
            ProductId = productId;
            Quantity = quantity;
        }
    }
}