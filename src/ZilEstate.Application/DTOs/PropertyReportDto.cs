using System.ComponentModel.DataAnnotations;

namespace ZilEstate.Application.DTOs;

public class CreatePropertyReportDto : IValidatableObject
{
    private static readonly HashSet<string> AllowedReasons = new(StringComparer.OrdinalIgnoreCase)
    {
        "Scam",
        "Duplicate listing",
        "Wrong price",
        "Inappropriate content",
        "Other",
    };

    [Range(1, int.MaxValue)]
    public int PropertyId { get; set; }

    [Required, MaxLength(100)]
    public string ReporterName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string ReporterEmail { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Details { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Reason) && !AllowedReasons.Contains(Reason))
        {
            yield return new ValidationResult(
                "Reason must be one of: Scam, Duplicate listing, Wrong price, Inappropriate content, or Other.",
                new[] { nameof(Reason) });
        }
    }
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

public class UpdateReportStatusDto : IValidatableObject
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Reviewed",
        "Dismissed",
    };

    [Required, MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Status) && !AllowedStatuses.Contains(Status))
        {
            yield return new ValidationResult(
                "Status must be one of: Pending, Reviewed, or Dismissed.",
                new[] { nameof(Status) });
        }
    }
}
