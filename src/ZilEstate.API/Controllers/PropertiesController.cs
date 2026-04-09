using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;
using ZilEstate.Domain.Enums;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly PropertyService _propertyService;
    private readonly IWebHostEnvironment _env;

    public PropertiesController(PropertyService propertyService, IWebHostEnvironment env)
    {
        _propertyService = propertyService;
        _env = env;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private bool IsAdmin() => User.IsInRole("Admin");

    [HttpGet]
    public async Task<IActionResult> GetProperties(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? bedrooms = null,
        [FromQuery] int? bathrooms = null,
        [FromQuery] int? locationId = null,
        [FromQuery] string? keyword = null,
        [FromQuery] string sortBy = "newest",
        [FromQuery] bool includeUnapproved = false,
        [FromQuery] string? rentalType = null,
        [FromQuery] double? lat = null,
        [FromQuery] double? lng = null,
        [FromQuery] double? radiusKm = null,
        [FromQuery] string? district = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 12;

        // Only admins can request unapproved listings
        if (includeUnapproved && !IsAdmin()) includeUnapproved = false;

        var result = await _propertyService.GetPropertiesAsync(
            page, pageSize, type, status, minPrice, maxPrice,
            bedrooms, bathrooms, locationId, keyword, sortBy, includeUnapproved, rentalType, district, cancellationToken);

        // Apply distance filter in memory if lat/lng/radiusKm provided
        if (lat.HasValue && lng.HasValue && radiusKm.HasValue)
        {
            result.Items = result.Items
                .Where(p => Haversine(lat.Value, lng.Value, p.Latitude, p.Longitude) <= radiusKm.Value)
                .ToList();
            result.TotalCount = result.Items.Count();
        }

        return Ok(result);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleApproval(int id, CancellationToken cancellationToken)
    {
        var success = await _propertyService.ToggleApprovalAsync(id, cancellationToken);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpPost("{id:int}/feature")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleFeatured(int id, CancellationToken cancellationToken)
    {
        var success = await _propertyService.ToggleFeaturedAsync(id, cancellationToken);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProperties(CancellationToken cancellationToken = default)
    {
        var result = await _propertyService.GetFeaturedPropertiesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("my-listings")]
    [Authorize]
    public async Task<IActionResult> GetMyListings(CancellationToken cancellationToken = default)
    {
        var result = await _propertyService.GetMyPropertiesAsync(GetCurrentUserId(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProperty(int id, CancellationToken cancellationToken = default)
    {
        PropertyDto? property;
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : (int?)null;
        if (userId.HasValue)
            property = await _propertyService.GetPropertyByIdForOwnerAsync(id, userId.Value, cancellationToken);
        else
            property = await _propertyService.GetPropertyByIdAsync(id, cancellationToken);

        if (property == null) return NotFound(new { message = "Property not found" });
        return Ok(property);
    }

    [HttpPost("{id:int}/close")]
    [Authorize]
    public async Task<IActionResult> CloseListing(int id, [FromBody] CloseListingDto dto, CancellationToken cancellationToken)
    {
        var success = await _propertyService.CloseListingAsync(id, GetCurrentUserId(), dto.Status, cancellationToken);
        if (!success) return NotFound(new { message = "Property not found or unauthorized" });
        return Ok();
    }

    [HttpGet("admin/users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _propertyService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost("admin/users/{userId:int}/verify")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleUserVerified(int userId, CancellationToken cancellationToken)
    {
        var success = await _propertyService.ToggleUserVerifiedAsync(userId, cancellationToken);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpPost("admin/users/{userId:int}/set-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUserRole(int userId, [FromQuery] string role, CancellationToken cancellationToken)
    {
        var success = await _propertyService.SetUserRoleAsync(userId, role, cancellationToken);
        if (!success) return BadRequest(new { message = "Invalid role or user not found" });
        return Ok();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProperty(
        [FromBody] CreatePropertyDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var property = await _propertyService.CreatePropertyAsync(dto, GetCurrentUserId(), cancellationToken);
        return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateProperty(
        int id,
        [FromBody] CreatePropertyDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var property = await _propertyService.UpdatePropertyAsync(id, dto, GetCurrentUserId(), IsAdmin(), cancellationToken);
        if (property == null) return NotFound(new { message = "Property not found or unauthorized" });
        return Ok(property);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteProperty(int id, CancellationToken cancellationToken = default)
    {
        var success = await _propertyService.DeletePropertyAsync(id, GetCurrentUserId(), IsAdmin(), cancellationToken);
        if (!success) return NotFound(new { message = "Property not found or unauthorized" });
        return NoContent();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        var uploadsDir = Path.Combine(_env.ContentRootPath, "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".mp4", ".mov", ".webm", ".avi", ".mkv" };
        if (!allowed.Contains(ext))
            return BadRequest(new { message = "Invalid file type. Allowed: images (jpg, png, webp, gif) and videos (mp4, mov, webm, avi, mkv)" });

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream, cancellationToken);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
        return Ok(url);
    }

    [HttpGet("{id}/similar")]
    public async Task<IActionResult> GetSimilar(int id, [FromQuery] int count = 4, CancellationToken cancellationToken = default) =>
        Ok(await _propertyService.GetSimilarAsync(id, count, cancellationToken));

    [HttpGet("{id}/analytics")]
    [Authorize]
    public async Task<IActionResult> GetAnalytics(int id, CancellationToken cancellationToken = default)
    {
        var result = await _propertyService.GetAnalyticsAsync(id, GetCurrentUserId(), IsAdmin(), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("{id}/promote")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TogglePromote(int id, CancellationToken cancellationToken = default) =>
        await _propertyService.TogglePromotedAsync(id, cancellationToken) ? Ok() : NotFound();

    [HttpPost("{id}/renew")]
    [Authorize]
    public async Task<IActionResult> Renew(int id, CancellationToken cancellationToken = default) =>
        await _propertyService.RenewListingAsync(id, GetCurrentUserId(), IsAdmin(), cancellationToken) ? Ok() : Forbid();

    [HttpPost("admin/bulk-approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkApprove([FromBody] BulkActionDto dto, CancellationToken cancellationToken = default) =>
        Ok(new { count = await _propertyService.BulkApproveAsync(dto.PropertyIds, cancellationToken) });

    [HttpPost("admin/bulk-reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkReject([FromBody] BulkActionDto dto, CancellationToken cancellationToken = default) =>
        Ok(new { count = await _propertyService.BulkRejectAsync(dto.PropertyIds, cancellationToken) });

    [HttpPost("admin/bulk-feature")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BulkFeature([FromBody] BulkActionDto dto, [FromQuery] bool feature = true, CancellationToken cancellationToken = default) =>
        Ok(new { count = await _propertyService.BulkFeatureAsync(dto.PropertyIds, feature, cancellationToken) });

    [HttpGet("{id:int}/price-history")]
    public async Task<IActionResult> GetPriceHistory(int id, CancellationToken cancellationToken)
    {
        var history = await _propertyService.GetPriceHistoryAsync(id, cancellationToken);
        return Ok(history);
    }

    [HttpPost("{id:int}/verify")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> VerifyProperty(int id, CancellationToken cancellationToken)
    {
        var success = await _propertyService.VerifyPropertyAsync(id, cancellationToken);
        if (!success) return NotFound();
        return Ok();
    }

    [HttpGet("{id:int}/score")]
    [Authorize]
    public async Task<IActionResult> GetListingScore(int id, CancellationToken cancellationToken)
    {
        var score = await _propertyService.GetListingScoreAsync(id, cancellationToken);
        return Ok(score);
    }

    // Property Tracking endpoints (Feature 5)
    [HttpPost("{id:int}/track")]
    [Authorize]
    public async Task<IActionResult> TrackProperty(int id, CancellationToken cancellationToken)
    {
        var success = await _propertyService.TrackPropertyAsync(id, GetCurrentUserId(), cancellationToken);
        if (!success) return NotFound(new { message = "Property not found" });
        return Ok();
    }

    [HttpDelete("{id:int}/track")]
    [Authorize]
    public async Task<IActionResult> UntrackProperty(int id, CancellationToken cancellationToken)
    {
        var success = await _propertyService.UntrackPropertyAsync(id, GetCurrentUserId(), cancellationToken);
        if (!success) return NotFound(new { message = "Tracking record not found" });
        return NoContent();
    }

    [HttpGet("tracked")]
    [Authorize]
    public async Task<IActionResult> GetTrackedProperties(CancellationToken cancellationToken)
    {
        var tracked = await _propertyService.GetTrackedPropertiesAsync(GetCurrentUserId(), cancellationToken);
        return Ok(tracked);
    }

    // Verification Details endpoint (Feature 8)
    [HttpGet("{id:int}/verification-details")]
    public async Task<IActionResult> GetVerificationDetails(int id, CancellationToken cancellationToken)
    {
        var property = await _propertyService.GetPropertyByIdAsync(id, cancellationToken);
        if (property == null) return NotFound(new { message = "Property not found" });

        return Ok(new
        {
            photosVerified = property.IsVerified,
            priceConfirmed = property.IsVerified,
            ownerIdVerified = property.IsVerified,
            verifiedAt = property.VerifiedAt?.ToString("o"),
            verifiedBy = property.IsVerified ? "ZilEstate Team" : "Not verified"
        });
    }

    [HttpGet("estimate")]
    public async Task<IActionResult> EstimatePrice(
        [FromQuery] int locationId,
        [FromQuery] double? sizeM2,
        [FromQuery] string type = "House",
        [FromQuery] string status = "ForSale",
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<PropertyType>(type, true, out var propType))
            return BadRequest(new { message = "Invalid property type" });
        if (!Enum.TryParse<PropertyStatus>(status, true, out var propStatus))
            return BadRequest(new { message = "Invalid property status" });

        var estimate = await _propertyService.EstimatePriceAsync(locationId, sizeM2, propType, propStatus, cancellationToken);
        return Ok(new { estimatedPrice = estimate, currency = "MUR" });
    }
}
