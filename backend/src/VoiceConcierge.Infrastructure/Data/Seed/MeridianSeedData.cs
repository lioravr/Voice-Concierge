using System.Text.Json;

namespace VoiceConcierge.Infrastructure.Data.Seed;

/// <summary>
/// Seed data loader for The Meridian Casino & Resort
/// Loads data from JSON files for easier maintenance
/// </summary>
public static class MeridianSeedData
{
    private static readonly string SeedDataPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Data", "Seed"
    );

    public static class VoiceConfigurations
    {
        private static List<VoiceConfigurationData>? _cachedVoices;

        public static List<VoiceConfigurationData> Voices
        {
            get
            {
                if (_cachedVoices != null)
                    return _cachedVoices;

                var jsonPath = Path.Combine(SeedDataPath, "voices.json");
                var jsonContent = File.ReadAllText(jsonPath);
                _cachedVoices = JsonSerializer.Deserialize<List<VoiceConfigurationData>>(
                    jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<VoiceConfigurationData>();

                return _cachedVoices;
            }
        }
    }

    public static class FAQs
    {
        private static List<FAQData>? _cachedFaqs;

        public static List<FAQData> Items
        {
            get
            {
                if (_cachedFaqs != null)
                    return _cachedFaqs;

                var jsonPath = Path.Combine(SeedDataPath, "faqs.json");
                var jsonContent = File.ReadAllText(jsonPath);
                _cachedFaqs = JsonSerializer.Deserialize<List<FAQData>>(
                    jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<FAQData>();

                return _cachedFaqs;
            }
        }
    }

    // Data models matching JSON structure
    public class VoiceConfigurationData
    {
        public int VoiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Accent { get; set; } = string.Empty;
        public string ProviderVoiceId { get; set; } = string.Empty;
    }

    public class FAQData
    {
        public string Category { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
