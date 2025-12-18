namespace api_joyeria.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order? Order { get; set; } // Navigation property

    public decimal Amount { get; set; } // Paid amount
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
}