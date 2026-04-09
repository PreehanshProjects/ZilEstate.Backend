using Microsoft.EntityFrameworkCore;
using ZilEstate.Application.Common.Interfaces;
using ZilEstate.Application.DTOs;
using ZilEstate.Domain.Entities;

namespace ZilEstate.Application.Services;

public class ReviewService
{
    private readonly IApplicationDbContext _context;

    public ReviewService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ── Reviews ──────────────────────────────────────────────────────────────

    public async Task<List<ReviewDto>> GetReviewsAsync(int propertyId, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                AuthorName = r.AuthorName,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
            })
            .ToListAsync(ct);
    }

    public async Task<ReviewDto> AddReviewAsync(int propertyId, CreateReviewDto dto, CancellationToken ct = default)
    {
        var review = new Review
        {
            PropertyId = propertyId,
            AuthorName = dto.AuthorName,
            Rating = dto.Rating,
            Comment = dto.Comment,
        };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(ct);

        return new ReviewDto
        {
            Id = review.Id,
            PropertyId = review.PropertyId,
            AuthorName = review.AuthorName,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
        };
    }

    // ── Questions ─────────────────────────────────────────────────────────────

    public async Task<List<QuestionDto>> GetQuestionsAsync(int propertyId, CancellationToken ct = default)
    {
        return await _context.Questions
            .Where(q => q.PropertyId == propertyId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                PropertyId = q.PropertyId,
                AuthorName = q.AuthorName,
                Question = q.Question,
                Answer = q.Answer,
                CreatedAt = q.CreatedAt,
                AnsweredAt = q.AnsweredAt,
            })
            .ToListAsync(ct);
    }

    public async Task<QuestionDto> AddQuestionAsync(int propertyId, CreateQuestionDto dto, CancellationToken ct = default)
    {
        var question = new PropertyQuestion
        {
            PropertyId = propertyId,
            AuthorName = dto.AuthorName,
            Question = dto.Question,
        };
        _context.Questions.Add(question);
        await _context.SaveChangesAsync(ct);

        return new QuestionDto
        {
            Id = question.Id,
            PropertyId = question.PropertyId,
            AuthorName = question.AuthorName,
            Question = question.Question,
            CreatedAt = question.CreatedAt,
        };
    }

    /// <summary>Property owner answers a question. Returns null if not found or not the owner.</summary>
    public async Task<QuestionDto?> AnswerQuestionAsync(int propertyId, int questionId, int userId, string answer, CancellationToken ct = default)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId, ct);

        if (property == null) return null;

        var question = await _context.Questions
            .FirstOrDefaultAsync(q => q.Id == questionId && q.PropertyId == propertyId, ct);

        if (question == null) return null;

        question.Answer = answer;
        question.AnsweredAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        return new QuestionDto
        {
            Id = question.Id,
            PropertyId = question.PropertyId,
            AuthorName = question.AuthorName,
            Question = question.Question,
            Answer = question.Answer,
            CreatedAt = question.CreatedAt,
            AnsweredAt = question.AnsweredAt,
        };
    }
}
