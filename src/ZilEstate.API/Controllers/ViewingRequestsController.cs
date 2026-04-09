using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViewingRequestsController : ControllerBase
{
    private readonly ViewingRequestService _service;
    public ViewingRequestsController(ViewingRequestService service) { _service = service; }
    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private bool IsAdmin() => User.IsInRole("Admin");

    [HttpPost("property/{propertyId}")]
    public async Task<IActionResult> Create(int propertyId, [FromBody] CreateViewingRequestDto dto, CancellationToken ct)
    {
        try
        {
            int? userId = User.Identity?.IsAuthenticated == true ? UserId() : null;
            var result = await _service.CreateAsync(propertyId, userId, dto, ct);
            return Ok(result);
        }
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

    [HttpPut("{id}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateViewingRequestStatusDto dto, CancellationToken ct) =>
        await _service.UpdateStatusAsync(id, UserId(), IsAdmin(), dto.Status, ct) ? Ok() : NotFound();
}
