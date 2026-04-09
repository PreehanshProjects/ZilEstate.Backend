using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiriesController : ControllerBase
{
    private readonly InquiryService _service;
    public InquiriesController(InquiryService service) { _service = service; }
    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private bool IsAdmin() => User.IsInRole("Admin");

    [HttpPost("property/{propertyId}")]
    public async Task<IActionResult> Create(int propertyId, [FromBody] CreateInquiryDto dto, CancellationToken ct)
    {
        try { return Ok(await _service.CreateAsync(propertyId, dto, ct)); }
        catch { return BadRequest(new { message = "Property not found" }); }
    }

    [HttpGet("property/{propertyId}")]
    [Authorize]
    public async Task<IActionResult> GetByProperty(int propertyId, CancellationToken ct) =>
        Ok(await _service.GetByPropertyAsync(propertyId, UserId(), IsAdmin(), ct));

    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken ct) =>
        Ok(await _service.GetByOwnerAsync(UserId(), ct));

    [HttpPost("{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        await _service.MarkReadAsync(id, ct);
        return Ok();
    }
}
