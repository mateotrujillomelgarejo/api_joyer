using System;
using api_joyeria.Domain.Enums;
using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Domain.Entities
{
    public sealed class Payment
    {
        public string Reference { get; private set; }
        public string OrderId { get; private set; }
        public Money Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        private Payment() { }

        public static Payment Create(string reference, string orderId, Money amount, PaymentStatus status)
        {
            if (string.IsNullOrWhiteSpace(reference)) throw new DomainException("Payment reference is required");
            if (string.IsNullOrWhiteSpace(orderId)) throw new DomainException("OrderId is required");
            if (amount == null) throw new DomainException("Amount is required");

            return new Payment
            {
                Reference = reference,
                OrderId = orderId,
                Amount = amount,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
        }

        public bool IsFinalized()
        {
            return Status == PaymentStatus.Completed || Status == PaymentStatus.Failed || Status == PaymentStatus.Cancelled;
        }

        public void MarkAsCompleted()
        {
            if (IsFinalized()) throw new DomainException("Payment already finalized");
            Status = PaymentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            if (IsFinalized()) throw new DomainException("Payment already finalized");
            Status = PaymentStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsCancelled()
        {
            if (IsFinalized()) throw new DomainException("Payment already finalized");
            Status = PaymentStatus.Cancelled;
            CompletedAt = DateTime.UtcNow;
        }
    }
}