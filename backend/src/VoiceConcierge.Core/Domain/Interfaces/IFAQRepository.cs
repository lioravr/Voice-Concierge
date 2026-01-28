using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Core.Domain.Interfaces;

public interface IFAQRepository
{
    Task<List<FAQ>> GetAllAsync();
    
    Task<FAQ?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Performs semantic search using vector similarity
    /// </summary>
    /// <param name="embedding">Query embedding vector</param>
    /// <param name="limit">Maximum number of results</param>
    /// <param name="threshold">Maximum distance threshold (lower is more similar)</param>
    /// <returns>List of FAQs ordered by similarity</returns>
    Task<List<(FAQ FAQ, double Distance)>> SearchByEmbeddingAsync(float[] embedding, int limit = 5, double threshold = 0.3);
    
    Task<FAQ> CreateAsync(FAQ faq);
    
    Task UpdateAsync(FAQ faq);
    
    Task DeleteAsync(Guid id);
}
