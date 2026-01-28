using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;

namespace VoiceConcierge.Infrastructure.Data.Repositories;

public class FAQRepository : IFAQRepository
{
    private readonly ApplicationDbContext _context;

    public FAQRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FAQ>> GetAllAsync()
    {
        return await _context.FAQs
            .OrderBy(f => f.Category)
            .ThenBy(f => f.Question)
            .ToListAsync();
    }

    public async Task<FAQ?> GetByIdAsync(Guid id)
    {
        return await _context.FAQs.FindAsync(id);
    }

    public async Task<List<(FAQ FAQ, double Distance)>> SearchByEmbeddingAsync(float[] embedding, int limit = 5, double threshold = 0.3)
    {
        // Convert float array to Vector
        var queryVector = new Vector(embedding);
        
        // Use cosine distance for semantic similarity search
        var results = await _context.FAQs
            .Where(f => f.Embedding != null)
            .Select(f => new
            {
                FAQ = f,
                Distance = f.Embedding!.CosineDistance(queryVector)
            })
            .Where(r => r.Distance <= threshold)
            .OrderBy(r => r.Distance)
            .Take(limit)
            .ToListAsync();

        return results.Select(r => (r.FAQ, r.Distance)).ToList();
    }

    public async Task<FAQ> CreateAsync(FAQ faq)
    {
        faq.Id = Guid.NewGuid();
        faq.CreatedAt = DateTime.UtcNow;
        faq.UpdatedAt = DateTime.UtcNow;

        _context.FAQs.Add(faq);
        await _context.SaveChangesAsync();

        return faq;
    }

    public async Task UpdateAsync(FAQ faq)
    {
        faq.UpdatedAt = DateTime.UtcNow;

        _context.FAQs.Update(faq);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var faq = await _context.FAQs.FindAsync(id);
        if (faq != null)
        {
            _context.FAQs.Remove(faq);
            await _context.SaveChangesAsync();
        }
    }
}
