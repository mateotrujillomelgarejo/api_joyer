namespace api_joyeria.Application.DTOs;

public class PaymentDto
{
    public int Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}