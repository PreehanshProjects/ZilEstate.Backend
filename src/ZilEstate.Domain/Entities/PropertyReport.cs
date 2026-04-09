namespace ZilEstate.Domain.Entities;

public class PropertyReport
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    public int? ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string ReporterEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty; // "Scam", "Duplicate", "WrongPrice", "Inappropriate", "Other"
    public string? Details { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Reviewed", "Dismissed"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
