using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace VoiceConcierge.Core.Services;

public class VoiceConfigurationService : IVoiceConfigurationService
{
    private readonly IVoiceConfigurationRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public VoiceConfigurationService(
        IVoiceConfigurationRepository repository,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
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

    public async Task<byte[]?> GeneratePreviewAsync(int voiceId)
    {
        // Get voice configuration
        var voice = await _repository.GetByVoiceIdAsync(voiceId);
        if (voice == null)
        {
            return null;
        }

        // Generate preview text based on voice personality
        var previewText = GetPreviewTextForVoice(voice.Name);

        // Call OpenAI TTS API
        var openAiApiKey = _configuration["OpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("OpenAI API key not configured");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/speech")
        {
            Headers =
            {
                { "Authorization", $"Bearer {openAiApiKey}" }
            },
            Content = JsonContent.Create(new
            {
                model = "tts-1",
                voice = voice.ProviderVoiceId,
                input = previewText
            })
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    private static string GetPreviewTextForVoice(string voiceName)
    {
        return voiceName.ToLower() switch
        {
            "james" => "Good evening. I'm James, your concierge at The Meridian. How may I assist you today?",
            "sofia" => "Hello! I'm Sofia, your voice concierge. I'd be delighted to help you with any questions about The Meridian.",
            "marcus" => "Hey there! I'm Marcus, your concierge assistant. What can I help you with at The Meridian today?",
            "elena" => "Welcome to The Meridian. I'm Elena, your voice concierge, here to assist you with any inquiries.",
            _ => "Welcome to The Meridian Casino and Resort. How may I assist you today?"
        };
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
