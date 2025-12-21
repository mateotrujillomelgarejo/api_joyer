using System;
using Xunit;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;
using api_joyeria.Domain.Enums;
using api_joyeria.Domain;

namespace api_joyeria.Tests.Domain
{
    public class PaymentTests
    {
        [Fact]
        public void CreatePayment_WithValidData_ShouldHavePendingStatus()
        {
            // Arrange
            var reference = Guid.NewGuid().ToString("N");
            var orderId = "order123";
            var amount = Money.Of(100m, "USD");

            // Act
            var payment = Payment.Create(reference, orderId, amount, PaymentStatus.Pending);

            // Assert
            Assert.Equal(reference, payment.Reference);
            Assert.Equal(orderId, payment.OrderId);
            Assert.Equal(100m, payment.Amount.Amount);
            Assert.Equal("Pending", payment.Status.ToString());
        }

        [Fact]
        public void MarkAsCompleted_WhenPending_ShouldSetCompletedAndTimestamp()
        {
            // Arrange
            var p = Payment.Create("ref1", "o1", Money.Of(10m, "USD"), PaymentStatus.Pending);

            // Act
            p.MarkAsCompleted();

            // Assert
            Assert.Equal("Completed", p.Status.ToString());
            Assert.NotNull(p.CompletedAt);
        }

        [Fact]
        public void MarkAsCompleted_WhenAlreadyFinalized_ShouldThrow()
        {
            // Arrange
            var p = Payment.Create("ref2", "o2", Money.Of(5m, "USD"), PaymentStatus.Completed);

            // Act & Assert
            Assert.Throws<DomainException>(() => p.MarkAsCompleted());
        }
    }
}