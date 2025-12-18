using api_joyeria.Domain.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public string GuestToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }

    public List<CartItem> Items { get; set; } = new();
}
