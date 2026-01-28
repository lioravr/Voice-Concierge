namespace VoiceConcierge.Core.Domain.Entities;

/// <summary>
/// Represents a voice personality configuration for the concierge
/// </summary>
public class VoiceConfiguration
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique identifier for the voice (1-4 for our voice options)
    /// </summary>
    public int VoiceId { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string Gender { get; set; } = string.Empty;
    
    public string Accent { get; set; } = string.Empty;
    
    /// <summary>
    /// Provider-specific voice identifier (e.g., OpenAI voice ID, ElevenLabs voice ID)
    /// </summary>
    public string ProviderVoiceId { get; set; } = string.Empty;
    
    /// <summary>
    /// Only one voice can be active at a time
    /// </summary>
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
