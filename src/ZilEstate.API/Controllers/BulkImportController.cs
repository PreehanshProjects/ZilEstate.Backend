using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Domain.Entities;
using ZilEstate.Domain.Enums;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/bulk-import")]
[Authorize]
public class BulkImportController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public BulkImportController(IApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>
    /// POST /api/bulk-import/properties
    /// CSV format: Title,Description,Price,Type,Status,Bedrooms,Bathrooms,SizeM2,District,SellerName,SellerPhone
    /// </summary>
    [HttpPost("properties")]
    public async Task<IActionResult> ImportProperties(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".csv")
            return BadRequest(new { message = "Only CSV files are supported" });

        string content;
        using (var reader = new StreamReader(file.OpenReadStream()))
            content = await reader.ReadToEndAsync(cancellationToken);

        var lines = content.Split('\n');
        var errors = new List<string>();
        var imported = 0;
        var userId = GetCurrentUserId();

        // Load all locations once
        var locations = await _context.Locations.ToListAsync(cancellationToken);

        // Skip header row (index 0)
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim('\r').Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var rowNum = i + 1;
            var cols = line.Split(',');

            if (cols.Length < 11)
            {
                errors.Add($"row {rowNum}: expected 11 columns, got {cols.Length}");
                continue;
            }

            var title       = cols[0].Trim();
            var description = cols[1].Trim();
            var priceStr    = cols[2].Trim();
            var typeStr     = cols[3].Trim();
            var statusStr   = cols[4].Trim();
            var bedroomsStr = cols[5].Trim();
            var bathroomsStr= cols[6].Trim();
            var sizeStr     = cols[7].Trim();
            var district    = cols[8].Trim();
            var sellerName  = cols[9].Trim();
            var sellerPhone = cols[10].Trim();

            if (string.IsNullOrEmpty(title))
            {
                errors.Add($"row {rowNum}: title is required");
                continue;
            }

            if (!decimal.TryParse(priceStr, out var price))
            {
                errors.Add($"row {rowNum}: invalid price '{priceStr}'");
                continue;
            }

            if (!Enum.TryParse<PropertyType>(typeStr, true, out var propType))
            {
                errors.Add($"row {rowNum}: invalid type '{typeStr}' (use House/Villa/Apartment/Land/Commercial)");
                continue;
            }

            if (!Enum.TryParse<PropertyStatus>(statusStr, true, out var propStatus))
            {
                errors.Add($"row {rowNum}: invalid status '{statusStr}' (use ForSale/ForRent)");
                continue;
            }

            var location = locations.FirstOrDefault(l =>
                l.District.Equals(district, StringComparison.OrdinalIgnoreCase) ||
                l.City.Equals(district, StringComparison.OrdinalIgnoreCase));

            if (location == null)
            {
                errors.Add($"row {rowNum}: district/city not found '{district}'");
                continue;
            }

            int? bedrooms = null;
            if (!string.IsNullOrEmpty(bedroomsStr) && int.TryParse(bedroomsStr, out var b))
                bedrooms = b;

            int? bathrooms = null;
            if (!string.IsNullOrEmpty(bathroomsStr) && int.TryParse(bathroomsStr, out var ba))
                bathrooms = ba;

            double? sizeM2 = null;
            if (!string.IsNullOrEmpty(sizeStr) && double.TryParse(sizeStr, out var sz))
                sizeM2 = sz;

            var property = new Property
            {
                Title = title,
                Description = string.IsNullOrEmpty(description) ? title : description,
                Price = price,
                Type = propType,
                Status = propStatus,
                Bedrooms = bedrooms,
                Bathrooms = bathrooms,
                SizeM2 = sizeM2,
                LocationId = location.Id,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                SellerName = string.IsNullOrEmpty(sellerName) ? "Importer" : sellerName,
                SellerPhone = string.IsNullOrEmpty(sellerPhone) ? "N/A" : sellerPhone,
                UserId = userId,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMonths(3)
            };

            _context.Properties.Add(property);
            imported++;
        }

        if (imported > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return Ok(new { imported, errors });
    }
}
