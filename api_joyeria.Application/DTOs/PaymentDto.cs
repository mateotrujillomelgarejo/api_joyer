namespace api_joyeria.Application.DTOs;

public class PaymentDto
{
    public string Method { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}