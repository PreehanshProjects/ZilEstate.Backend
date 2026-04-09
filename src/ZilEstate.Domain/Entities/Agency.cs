namespace ZilEstate.Domain.Entities;

public class Agency
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Plan { get; set; } = "Free"; // Free, Professional, Premium
    public int? AverageResponseHours { get; set; }
    public ICollection<AgencyReview> Reviews { get; set; } = new List<AgencyReview>();

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<User> Agents { get; set; } = new List<User>();
}
