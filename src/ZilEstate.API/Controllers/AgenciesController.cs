using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgenciesController : ControllerBase
{
    private readonly AgencyService _agencyService;

    public AgenciesController(AgencyService agencyService)
    {
        _agencyService = agencyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AgencyDto>>> GetAll()
    {
        return await _agencyService.GetAllAgenciesAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AgencyDetailDto>> GetById(int id)
    {
        var agency = await _agencyService.GetAgencyByIdAsync(id);
        if (agency == null) return NotFound();
        return agency;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AgencyDto>> Create(CreateAgencyDto dto)
    {
        var agency = await _agencyService.CreateAgencyAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = agency.Id }, agency);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AgencyDto>> Update(int id, CreateAgencyDto dto)
    {
        // Security: Check if user belongs to this agency OR is admin
        var userAgencyId = User.FindFirst("AgencyId")?.Value;
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && (string.IsNullOrEmpty(userAgencyId) || int.Parse(userAgencyId) != id))
        {
            return Forbid();
        }

        var agency = await _agencyService.UpdateAgencyAsync(id, dto);
        if (agency == null) return NotFound();
        return agency;
    }

    [HttpGet("{id}/agents")]
    public async Task<IActionResult> GetAgents(int id) =>
        Ok(await _agencyService.GetAgentsAsync(id));

    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(int id) =>
        Ok(await _agencyService.GetReviewsAsync(id));

    [HttpPost("{id}/reviews")]
    public async Task<IActionResult> AddReview(int id, [FromBody] CreateAgencyReviewDto dto) =>
        Ok(await _agencyService.AddReviewAsync(id, dto));

    [HttpPut("{id}/plan")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePlan(int id, [FromQuery] string plan) =>
        await _agencyService.UpdatePlanAsync(id, plan) ? Ok() : NotFound();

    [HttpPut("{id}/response-time")]
    [Authorize]
    public async Task<IActionResult> UpdateResponseTime(int id, [FromQuery] int hours)
    {
        var userAgencyId = User.FindFirst("AgencyId")?.Value;
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && (string.IsNullOrEmpty(userAgencyId) || int.Parse(userAgencyId) != id))
            return Forbid();

        return await _agencyService.UpdateResponseTimeAsync(id, hours) ? Ok() : NotFound();
    }
}
