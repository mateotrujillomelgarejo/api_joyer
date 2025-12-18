namespace api_joyeria.Domain.Entities;

public class GuestToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime Expiry { get; set; }
}