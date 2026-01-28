using Pgvector;

namespace VoiceConcierge.Core.Domain.Entities;

/// <summary>
/// Represents a Frequently Asked Question with semantic search capability
/// </summary>
public class FAQ
{
    public Guid Id { get; set; }
    
    public string Question { get; set; } = string.Empty;
    
    public string Answer { get; set; } = string.Empty;
    
    public string? Category { get; set; }
    
    /// <summary>
    /// Vector embedding for semantic search (1536 dimensions for OpenAI embeddings)
    /// </summary>
    public Vector? Embedding { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
