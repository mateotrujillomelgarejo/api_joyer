namespace api_joyeria.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public string GuestToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiredAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}