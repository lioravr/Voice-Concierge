using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.Infrastructure.Services;

public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _client;
    private const string Model = "text-embedding-3-small"; // 1536 dimensions
    
    public OpenAIEmbeddingService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"] 
            ?? throw new InvalidOperationException("OpenAI API key not configured");
            
        _client = new EmbeddingClient(Model, apiKey);
    }
    
    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }
        
        var embedding = await _client.GenerateEmbeddingAsync(text);
        return embedding.Value.Vector.ToArray();
    }
}
