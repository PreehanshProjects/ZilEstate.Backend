namespace ZilEstate.Application.DTOs;

public class OpenHouseEventDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string? Description { get; set; }
    public int? MaxAttendees { get; set; }
    public int RsvpCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OpenHouseRsvpDto> Rsvps { get; set; } = new();
}

public class OpenHouseRsvpDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateOpenHouseEventDto
{
    public DateTime EventDate { get; set; }
    public string? Description { get; set; }
    public int? MaxAttendees { get; set; }
}

public class CreateOpenHouseRsvpDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
