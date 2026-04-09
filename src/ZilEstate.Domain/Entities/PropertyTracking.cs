namespace ZilEstate.Domain.Entities;

public class PropertyTracking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public decimal TrackedPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
