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
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class UpdateFAQDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class FAQSearchRequest
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 5;
    public double Threshold { get; set; } = 0.3;
}

public class FAQSearchResult
{
    public FAQDto FAQ { get; set; } = null!;
    public double Distance { get; set; }
    public double Similarity => 1 - Distance; // Convert distance to similarity score
}
