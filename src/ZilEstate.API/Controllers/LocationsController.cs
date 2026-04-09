using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public LocationsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations(CancellationToken cancellationToken = default)
    {
        var locations = await _context.Locations
            .OrderBy(l => l.District).ThenBy(l => l.City)
            .Select(l => new { l.Id, l.District, l.City, l.Latitude, l.Longitude })
            .ToListAsync(cancellationToken);

        return Ok(locations);
    }
}
