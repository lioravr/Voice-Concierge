using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Core.Domain.Interfaces;

public interface IVoiceConfigurationRepository
{
    Task<List<VoiceConfiguration>> GetAllAsync();
    
    Task<VoiceConfiguration?> GetActiveAsync();
    
    Task<VoiceConfiguration?> GetByVoiceIdAsync(int voiceId);
    
    /// <summary>
    /// Sets the specified voice as active and deactivates all others
    /// </summary>
    Task SetActiveAsync(int voiceId);
}
