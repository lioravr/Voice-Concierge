using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;

namespace VoiceConcierge.Infrastructure.Data.Repositories;

public class VoiceConfigurationRepository : IVoiceConfigurationRepository
{
    private readonly ApplicationDbContext _context;

    public VoiceConfigurationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VoiceConfiguration>> GetAllAsync()
    {
        return await _context.VoiceConfigurations
            .OrderBy(v => v.VoiceId)
            .ToListAsync();
    }

    public async Task<VoiceConfiguration?> GetActiveAsync()
    {
        return await _context.VoiceConfigurations
            .FirstOrDefaultAsync(v => v.IsActive);
    }

    public async Task<VoiceConfiguration?> GetByVoiceIdAsync(int voiceId)
    {
        return await _context.VoiceConfigurations
            .FirstOrDefaultAsync(v => v.VoiceId == voiceId);
    }

    public async Task SetActiveAsync(int voiceId)
    {
        // Deactivate all voices
        var allVoices = await _context.VoiceConfigurations.ToListAsync();
        foreach (var voice in allVoices)
        {
            voice.IsActive = false;
        }

        // Activate the selected voice
        var selectedVoice = allVoices.FirstOrDefault(v => v.VoiceId == voiceId);
        if (selectedVoice == null)
        {
            throw new InvalidOperationException($"Voice configuration with VoiceId {voiceId} not found.");
        }

        selectedVoice.IsActive = true;
        await _context.SaveChangesAsync();
    }
}
