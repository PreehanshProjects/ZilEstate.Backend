namespace ZilEstate.Domain.Entities;

public class OpenHouseRsvp
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public OpenHouseEvent Event { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
