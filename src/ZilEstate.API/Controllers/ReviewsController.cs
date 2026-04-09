using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZilEstate.Application.DTOs;
using ZilEstate.Application.Services;

namespace ZilEstate.API.Controllers;

[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviewService;

    public ReviewsController(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // ── Reviews ──────────────────────────────────────────────────────────────

    [HttpGet("properties/{propertyId:int}/reviews")]
    public async Task<IActionResult> GetReviews(int propertyId, CancellationToken ct)
    {
        var reviews = await _reviewService.GetReviewsAsync(propertyId, ct);
        return Ok(reviews);
    }

    [HttpPost("properties/{propertyId:int}/reviews")]
    public async Task<IActionResult> AddReview(int propertyId, [FromBody] CreateReviewDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var review = await _reviewService.AddReviewAsync(propertyId, dto, ct);
        return CreatedAtAction(nameof(GetReviews), new { propertyId }, review);
    }

    // ── Questions ─────────────────────────────────────────────────────────────

    [HttpGet("properties/{propertyId:int}/questions")]
    public async Task<IActionResult> GetQuestions(int propertyId, CancellationToken ct)
    {
        var questions = await _reviewService.GetQuestionsAsync(propertyId, ct);
        return Ok(questions);
    }

    [HttpPost("properties/{propertyId:int}/questions")]
    public async Task<IActionResult> AddQuestion(int propertyId, [FromBody] CreateQuestionDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var question = await _reviewService.AddQuestionAsync(propertyId, dto, ct);
        return CreatedAtAction(nameof(GetQuestions), new { propertyId }, question);
    }

    [HttpPost("properties/{propertyId:int}/questions/{questionId:int}/answer")]
    [Authorize]
    public async Task<IActionResult> AnswerQuestion(int propertyId, int questionId, [FromBody] AnswerQuestionDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var result = await _reviewService.AnswerQuestionAsync(propertyId, questionId, userId, dto.Answer, ct);
        if (result == null) return NotFound(new { message = "Question not found or you are not the property owner" });
        return Ok(result);
    }
}
