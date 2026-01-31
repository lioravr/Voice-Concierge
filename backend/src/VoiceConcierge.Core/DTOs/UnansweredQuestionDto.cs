using System.ComponentModel.DataAnnotations;

namespace VoiceConcierge.Core.DTOs;

public class UnansweredQuestionDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public int Frequency { get; set; }
    public DateTime FirstAskedAt { get; set; }
    public DateTime LastAskedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RecordQuestionRequest
{
    [Required(ErrorMessage = "Question is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Question must be between 5 and 500 characters")]
    public string Question { get; set; } = string.Empty;
}

public class ConvertToFAQRequest
{
    [Required(ErrorMessage = "Answer is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Answer must be between 10 and 2000 characters")]
    public string Answer { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
}
