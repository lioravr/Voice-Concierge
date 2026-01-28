namespace VoiceConcierge.Core.Services;

/// <summary>
/// Service for generating text embeddings using OpenAI
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates an embedding vector for the given text
    /// </summary>
    /// <param name="text">Text to generate embedding for</param>
    /// <returns>Embedding vector as float array</returns>
    Task<float[]> GenerateEmbeddingAsync(string text);
}
