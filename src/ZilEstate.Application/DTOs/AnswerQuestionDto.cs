using System.ComponentModel.DataAnnotations;

namespace ZilEstate.Application.DTOs;

public class AnswerQuestionDto
{
    [Required, MaxLength(2000)]
    public string Answer { get; set; } = string.Empty;
}
