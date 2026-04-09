using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenHouseController : ControllerBase
{
    private readonly OpenHouseService _service;

    public OpenHouseController(OpenHouseService service)
    {
        _service = service;
    }

    [HttpGet("property/{propertyId:int}")]
    public async Task<IActionResult> GetByProperty(int propertyId, CancellationToken ct)
        => Ok(await _service.GetByPropertyAsync(propertyId, ct));

    [HttpPost("property/{propertyId:int}")]
    [Authorize]
    public async Task<IActionResult> Create(int propertyId, [FromBody] CreateOpenHouseEventDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(propertyId, dto, ct));

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{id:int}/rsvp")]
    public async Task<IActionResult> Rsvp(int id, [FromBody] CreateOpenHouseRsvpDto dto, CancellationToken ct)
    {
        var rsvp = await _service.RsvpAsync(id, dto, ct);
        return rsvp == null ? BadRequest("Event is full or not found.") : Ok(rsvp);
    }
}
