namespace ZilEstate.Domain.Entities;

public class OpenHouseEvent
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string? Description { get; set; }
    public int? MaxAttendees { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<OpenHouseRsvp> Rsvps { get; set; } = new List<OpenHouseRsvp>();
}
