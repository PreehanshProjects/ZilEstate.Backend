using System.ComponentModel.DataAnnotations;

namespace ZilEstate.Application.DTOs;

public class CloseListingDto
{
    /// <summary>"Sold" or "Rented"</summary>
    [Required]
    public string Status { get; set; } = "Sold";
}
