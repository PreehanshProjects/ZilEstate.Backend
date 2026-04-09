namespace ZilEstate.Domain.Entities;

public class PriceAlert
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string? District { get; set; }
    public string? PropertyType { get; set; }
    public decimal MaxPrice { get; set; }
    public string Status { get; set; } = "ForSale";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
