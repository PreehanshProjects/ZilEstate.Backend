namespace ZilEstate.Application.DTOs;

public class CreatePropertyReportDto
{
    public int PropertyId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string ReporterEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty; // "Scam", "Duplicate", "WrongPrice", "Inappropriate", "Other"
    public string? Details { get; set; }
}

public class PropertyReportDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public int? ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public string ReporterEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}

public class UpdateReportStatusDto
{
    public string Status { get; set; } = string.Empty;
}
