namespace VoiceConcierge.Core.DTOs;

public class VoiceConfigurationDto
{
    public Guid Id { get; set; }
    public int VoiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Gender { get; set; }
    public string? Accent { get; set; }
    public string? ProviderVoiceId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
