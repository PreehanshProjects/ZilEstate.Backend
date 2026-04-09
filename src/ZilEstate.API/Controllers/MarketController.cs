using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly MarketService _marketService;

    public MarketController(MarketService marketService)
    {
        _marketService = marketService;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    /// <summary>Get market overview: totals, averages, district and type breakdowns</summary>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var overview = await _marketService.GetOverviewAsync(cancellationToken);
        return Ok(overview);
    }

    /// <summary>Get price trends over time, optionally filtered by district</summary>
    [HttpGet("trends")]
    public async Task<IActionResult> GetTrends(
        [FromQuery] int months = 12,
        [FromQuery] string? district = null,
        CancellationToken cancellationToken = default)
    {
        if (months < 1 || months > 36) months = 12;
        var trends = await _marketService.GetPriceTrendsAsync(months, district, cancellationToken);
        return Ok(trends);
    }

    /// <summary>Get detailed stats for a specific district</summary>
    [HttpGet("district/{district}")]
    public async Task<IActionResult> GetDistrictDetail(string district, CancellationToken cancellationToken)
    {
        var detail = await _marketService.GetDistrictDetailAsync(district, cancellationToken);
        if (detail == null) return NotFound(new { message = $"No data found for district '{district}'" });
        return Ok(detail);
    }

    /// <summary>Get price distribution buckets</summary>
    [HttpGet("distribution")]
    public async Task<IActionResult> GetDistribution(
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var distribution = await _marketService.GetPriceDistributionAsync(status, cancellationToken);
        return Ok(distribution);
    }

    /// <summary>Get recent market activity (new this week vs last week)</summary>
    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity(CancellationToken cancellationToken)
    {
        var activity = await _marketService.GetRecentActivityAsync(cancellationToken);
        return Ok(activity);
    }

    /// <summary>Get most viewed properties</summary>
    [HttpGet("most-viewed")]
    public async Task<IActionResult> GetMostViewed(
        [FromQuery] int count = 8,
        CancellationToken cancellationToken = default)
    {
        if (count < 1 || count > 20) count = 8;
        var mostViewed = await _marketService.GetMostViewedAsync(count, cancellationToken);
        return Ok(mostViewed);
    }

    /// <summary>Create a price alert (auth required)</summary>
    [HttpPost("alerts")]
    [Authorize]
    public async Task<IActionResult> CreateAlert([FromBody] CreatePriceAlertDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();
        var alert = await _marketService.CreatePriceAlertAsync(userId, dto, cancellationToken);
        return Ok(alert);
    }

    /// <summary>Get current user's price alerts (auth required)</summary>
    [HttpGet("alerts")]
    [Authorize]
    public async Task<IActionResult> GetAlerts(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();
        var alerts = await _marketService.GetUserPriceAlertsAsync(userId, cancellationToken);
        return Ok(alerts);
    }

    /// <summary>Delete a price alert (auth required)</summary>
    [HttpDelete("alerts/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAlert(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();
        var deleted = await _marketService.DeletePriceAlertAsync(userId, id, cancellationToken);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("estimate")]
    public async Task<IActionResult> GetEstimate(
        [FromQuery] string district,
        [FromQuery] string type,
        [FromQuery] double? sizeM2 = null,
        [FromQuery] string status = "ForSale",
        CancellationToken cancellationToken = default)
    {
        var result = await _marketService.GetPriceEstimateAsync(district, type, sizeM2, status, cancellationToken);
        return Ok(result);
    }

    [HttpGet("recently-sold")]
    public async Task<IActionResult> GetRecentlySold([FromQuery] int count = 6, CancellationToken cancellationToken = default)
    {
        var result = await _marketService.GetRecentlySoldAsync(count, cancellationToken);
        return Ok(result);
    }
}
