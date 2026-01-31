using System.ComponentModel.DataAnnotations;

namespace VoiceConcierge.Core.DTOs;

public class FAQDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateFAQDto
{
    [Required(ErrorMessage = "Question is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Question must be between 5 and 500 characters")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Answer must be between 10 and 2000 characters")]
    public string Answer { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
}

public class UpdateFAQDto
{
    [Required(ErrorMessage = "Question is required")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Question must be between 5 and 500 characters")]
    public string Question { get; set; } = string.Empty;

    [Required(ErrorMessage = "Answer is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Answer must be between 10 and 2000 characters")]
    public string Answer { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
}

public class FAQSearchRequest
{
    [Required(ErrorMessage = "Query is required")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Query must be between 1 and 500 characters")]
    public string Query { get; set; } = string.Empty;

    [Range(1, 50, ErrorMessage = "Limit must be between 1 and 50")]
    public int Limit { get; set; } = 5;

    [Range(0.0, 1.0, ErrorMessage = "Threshold must be between 0 and 1")]
    public double Threshold { get; set; } = 0.3;
}

public class FAQSearchResult
{
    public FAQDto FAQ { get; set; } = null!;
    public double Distance { get; set; }
    public double Similarity => 1 - Distance; // Convert distance to similarity score
}
