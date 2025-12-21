using System;
using Xunit;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;
using api_joyeria.Domain;

namespace api_joyeria.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void CreateGuestOrder_WithValidData_ShouldCalculateTotalAndSetPending()
        {
            // Arrange
            var id = Guid.NewGuid().ToString("N");
            var email = "guest@example.com";
            var shipping = new ShippingAddress("John Doe", "Line1", "Line2", "City", "12345", "Country");

            var item1 = new OrderItem("P1", 2, Money.Of(10m, "USD"));
            var item2 = new OrderItem("P2", 1, Money.Of(5m, "USD"));

            // Act
            var order = Order.CreateGuestOrder(id, email, shipping, new[] { item1, item2 });

            // Assert
            Assert.Equal(id, order.Id);
            Assert.Equal(3, order.Items.Count);
            Assert.Equal("Pending", order.Status.ToString());
            Assert.Equal(25m, order.TotalAmount.Amount);
            Assert.Equal("USD", order.TotalAmount.Currency);
        }

        [Fact]
        public void MarkAsPaid_FromPending_ShouldSetPaid()
        {
            // Arrange
            var order = CreateSimplePendingOrder();

            // Act
            order.MarkAsPaid();

            // Assert
            Assert.Equal("Paid", order.Status.ToString());
        }

        [Fact]
        public void MarkAsPaid_FromCancelled_ShouldThrowDomainException()
        {
            // Arrange
            var order = CreateSimplePendingOrder();
            order.Cancel();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.MarkAsPaid());
        }

        [Fact]
        public void Cancel_FromPaid_ShouldThrowDomainException()
        {
            // Arrange
            var order = CreateSimplePendingOrder();
            order.MarkAsPaid();

            // Act & Assert
            Assert.Throws<DomainException>(() => order.Cancel());
        }

        private static Order CreateSimplePendingOrder()
        {
            var id = Guid.NewGuid().ToString("N");
            var shipping = new ShippingAddress("John Doe", "Line1", null, "City", "12345", "Country");
            var item = new OrderItem("P1", 1, Money.Of(10m, "USD"));
            return Order.CreateGuestOrder(id, "guest@example.com", shipping, new[] { item });
        }
    }
}