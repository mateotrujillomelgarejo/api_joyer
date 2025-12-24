using System.Collections.Generic;

namespace api_joyeria.Domain.Entities
{
    public sealed class Cart
    {
        private readonly List<CartItem> _items = new();

        public string Id { get; private set; }
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public Cart(string id)
        {
            Id = id;
        }

        public void AddItem(CartItem item)
        {
            if (item == null) throw new DomainException("Cart item is null");
            if (item.CartId != this.Id)
                throw new DomainException("CartItem cartId mismatch with Cart.Id");

            var existing = _items.Find(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.IncreaseQuantity(item.Quantity);
            }
            else
            {
                _items.Add(item);
            }
        }


        public void UpdateItemQuantity(string productId, int newQuantity)
        {
            var existing = _items.Find(i => i.ProductId == productId);
            if (existing == null) throw new DomainException("Item not found in cart");
            if (newQuantity <= 0) _items.Remove(existing);
            else existing.SetQuantity(newQuantity);
        }

        public void RemoveItem(string productId)
        {
            var existing = _items.Find(i => i.ProductId == productId);
            if (existing != null) _items.Remove(existing);
        }
    }
}