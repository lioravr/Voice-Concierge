namespace VoiceConcierge.Core.Domain.Entities;

/// <summary>
/// Represents a question that the system could not answer
/// </summary>
public class UnansweredQuestion
{
    public Guid Id { get; set; }
    
    public string Question { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of times this question has been asked
    /// </summary>
    public int Frequency { get; set; } = 1;
    
    public DateTime FirstAskedAt { get; set; }
    
    public DateTime LastAskedAt { get; set; }
    
    /// <summary>
    /// Status: pending, converted, dismissed
    /// </summary>
    public string Status { get; set; } = "pending";
}
