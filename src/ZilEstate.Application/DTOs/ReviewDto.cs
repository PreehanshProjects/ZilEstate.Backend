using System.ComponentModel.DataAnnotations;

namespace ZilEstate.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    [Required, MaxLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    [Required, Range(1, 5)]
    public int Rating { get; set; }

    [Required, MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
}

public class QuestionDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }
}

public class CreateQuestionDto
{
    [Required, MaxLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Question { get; set; } = string.Empty;
}
