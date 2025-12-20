namespace api_joyeria.Domain.Entities;

public class GuestToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
}