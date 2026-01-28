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
    public string Question { get; set; } = string.Empty;
}

public class ConvertToFAQRequest
{
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
}
