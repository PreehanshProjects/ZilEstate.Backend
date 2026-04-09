namespace ZilEstate.Application.DTOs;

public class AgencyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsVerified { get; set; }
    public int PropertyCount { get; set; }
    public string Plan { get; set; } = "Free";
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int? AverageResponseHours { get; set; }
}

public class AgencyDetailDto : AgencyDto
{
    public List<PropertyListDto> Properties { get; set; } = new();
}

public class CreateAgencyDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
}
