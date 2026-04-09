using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SavedSearchesController : ControllerBase
{
    private readonly SavedSearchService _service;
    public SavedSearchesController(SavedSearchService service) { _service = service; }
    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(await _service.GetByUserAsync(UserId(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSavedSearchDto dto, CancellationToken ct) =>
        Ok(await _service.CreateAsync(UserId(), dto, ct));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
        await _service.DeleteAsync(UserId(), id, ct) ? NoContent() : NotFound();
}
