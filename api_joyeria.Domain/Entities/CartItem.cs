namespace api_joyeria.Domain.Entities
{
    public sealed class CartItem
    {
        public string Id { get; private set; }

        public string CartId { get; private set; }   // 🔥 FK EXPLÍCITA
        public string ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { }

        public CartItem(string cartId, string productId, int quantity)
        {
            Id = Guid.NewGuid().ToString("N");

            if (string.IsNullOrWhiteSpace(cartId)) throw new DomainException("CartId required");
            if (string.IsNullOrWhiteSpace(productId)) throw new DomainException("ProductId required");
            if (quantity <= 0) throw new DomainException("Quantity must be greater than zero");

            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }

        internal void IncreaseQuantity(int amount)
        {
            if (amount <= 0) throw new DomainException("Increase amount must be positive");
            Quantity += amount;
        }

        internal void SetQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new DomainException("Quantity must be greater than zero");

            Quantity = newQuantity;
        }
    }
}
