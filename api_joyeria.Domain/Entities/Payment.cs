using System;

namespace api_joyeria.Domain.Entities;

public enum PaymentStatus { Pending, Succeeded, Failed, Cancelled, Refunded }

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }

    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty; // gateway id
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? IdempotencyKey { get; set; } // prevent duplicates
    public string? GatewayResponse { get; set; } // raw response for audit
}