using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public interface IVoiceConfigurationService
{
    Task<List<VoiceConfigurationDto>> GetAllAsync();
    Task<VoiceConfigurationDto?> GetActiveAsync();
    Task<VoiceConfigurationDto?> GetByVoiceIdAsync(int voiceId);
    Task SetActiveAsync(int voiceId);
    Task<byte[]?> GeneratePreviewAsync(int voiceId);
}
