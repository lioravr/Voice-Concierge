using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public class VoiceConfigurationService : IVoiceConfigurationService
{
    private readonly IVoiceConfigurationRepository _repository;

    public VoiceConfigurationService(IVoiceConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VoiceConfigurationDto>> GetAllAsync()
    {
        var voices = await _repository.GetAllAsync();
        return voices.Select(MapToDto).ToList();
    }

    public async Task<VoiceConfigurationDto?> GetActiveAsync()
    {
        var voice = await _repository.GetActiveAsync();
        return voice != null ? MapToDto(voice) : null;
    }

    public async Task<VoiceConfigurationDto?> GetByVoiceIdAsync(int voiceId)
    {
        var voice = await _repository.GetByVoiceIdAsync(voiceId);
        return voice != null ? MapToDto(voice) : null;
    }

    public async Task SetActiveAsync(int voiceId)
    {
        await _repository.SetActiveAsync(voiceId);
    }

    private static VoiceConfigurationDto MapToDto(VoiceConfiguration voice)
    {
        return new VoiceConfigurationDto
        {
            Id = voice.Id,
            VoiceId = voice.VoiceId,
            Name = voice.Name,
            Description = voice.Description,
            Gender = voice.Gender,
            Accent = voice.Accent,
            ProviderVoiceId = voice.ProviderVoiceId,
            IsActive = voice.IsActive,
            CreatedAt = voice.CreatedAt
        };
    }
}
