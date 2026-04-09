using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionsController : ControllerBase
{
    private readonly CollectionService _service;

    public CollectionsController(CollectionService service)
    {
        _service = service;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetMyCollections(CancellationToken ct)
        => Ok(await _service.GetUserCollectionsAsync(GetUserId(), ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCollection(int id, CancellationToken ct)
    {
        var c = await _service.GetByIdAsync(id, GetUserId(), ct);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCollectionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(GetUserId(), dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, GetUserId(), ct);
        return ok ? Ok() : NotFound();
    }

    [HttpPost("{id:int}/properties/{propertyId:int}")]
    public async Task<IActionResult> AddProperty(int id, int propertyId, CancellationToken ct)
    {
        var ok = await _service.AddPropertyAsync(id, propertyId, GetUserId(), ct);
        return ok ? Ok() : NotFound();
    }

    [HttpDelete("{id:int}/properties/{propertyId:int}")]
    public async Task<IActionResult> RemoveProperty(int id, int propertyId, CancellationToken ct)
    {
        var ok = await _service.RemovePropertyAsync(id, propertyId, GetUserId(), ct);
        return ok ? Ok() : NotFound();
    }
}
