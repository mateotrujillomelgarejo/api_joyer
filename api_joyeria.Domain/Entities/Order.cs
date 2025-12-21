using System;
using System.Collections.Generic;
using System.Linq;
using api_joyeria.Domain.Enums;
using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Domain.Entities
{
    public sealed class Order
    {
        private readonly List<OrderItem> _items = new();

        public string Id { get; private set; }
        public OrderCustomer Customer { get; private set; }
        public ShippingAddress ShippingAddress { get; private set; }
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public OrderStatus Status { get; private set; }
        public Money TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Order() { /* for ORM / serializers if needed */ }

        // Factory: crear orden para guest
        public static Order CreateGuestOrder(string id, string customerEmail, ShippingAddress shipping, IEnumerable<OrderItem> items)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new DomainException("Order id is required");
            if (string.IsNullOrWhiteSpace(customerEmail)) throw new DomainException("Customer email is required");
            if (items == null) throw new DomainException("Items are required");

            var itemList = items.ToList();
            if (!itemList.Any()) throw new DomainException("Order must contain at least one item");

            var order = new Order
            {
                Id = id,
                Customer = OrderCustomer.CreateGuest(customerEmail),
                ShippingAddress = shipping ?? throw new DomainException("Shipping address is required"),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var it in itemList)
            {
                order.AddItem(it);
            }

            order.RecalculateTotal();
            return order;
        }

        private void AddItem(OrderItem item)
        {
            if (item == null) throw new DomainException("Order item cannot be null");
            if (item.Quantity <= 0) throw new DomainException("Item quantity must be greater than zero");
            _items.Add(item);
        }

        private void RecalculateTotal()
        {
            if (!_items.Any()) TotalAmount = Money.Zero("USD");
            else
            {
                var currency = _items.First().UnitPrice.Currency;
                var sum = _items[0].Total;
                for (int i = 1; i < _items.Count; i++)
                    sum = sum.Add(_items[i].Total);

                TotalAmount = sum;
            }
        }

        // Behavior: validar si se puede pagar
        public bool IsPaymentAllowed()
        {
            return Status == OrderStatus.Pending;
        }

        // Marca la orden como pagada (valida transición)
        public void MarkAsPaid()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException($"Cannot mark order {Id} as paid from status {Status}");

            Status = OrderStatus.Paid;
        }

        public void MarkAsPaymentFailed()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException($"Cannot mark order {Id} as payment failed from status {Status}");

            Status = OrderStatus.PaymentFailed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Paid)
                throw new DomainException("Cannot cancel a paid order");

            Status = OrderStatus.Cancelled;
        }
    }
}