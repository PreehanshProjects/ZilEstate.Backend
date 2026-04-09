namespace ZilEstate.Application.DTOs;

public class SavedSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public string? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Bedrooms { get; set; }
    public string? District { get; set; }
    public string? Keyword { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSavedSearchDto
{
    public string Name { get; set; } = string.Empty;
    public string? PropertyType { get; set; }
    public string? Status { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Bedrooms { get; set; }
    public string? District { get; set; }
    public string? Keyword { get; set; }
}

public class ViewingRequestDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime PreferredDate { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateViewingRequestDto
{
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public DateTime PreferredDate { get; set; }
    public string? Message { get; set; }
}

public class InquiryDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInquiryDto
{
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal? Budget { get; set; }
}

public class AgencyReviewDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateAgencyReviewDto
{
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class PropertyAnalyticsDto
{
    public int PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int InquiryCount { get; set; }
    public int ViewingRequestCount { get; set; }
    public int ReviewCount { get; set; }
    public int QuestionCount { get; set; }
    public decimal? AverageRating { get; set; }
}

public class BulkActionDto
{
    public List<int> PropertyIds { get; set; } = new();
}

public class UpdateViewingRequestStatusDto
{
    public string Status { get; set; } = string.Empty; // Confirmed or Declined
}
