using Pgvector;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public class FAQService : IFAQService
{
    private readonly IFAQRepository _faqRepository;
    private readonly IEmbeddingService _embeddingService;

    public FAQService(IFAQRepository faqRepository, IEmbeddingService embeddingService)
    {
        _faqRepository = faqRepository;
        _embeddingService = embeddingService;
    }

    public async Task<List<FAQDto>> GetAllAsync()
    {
        var faqs = await _faqRepository.GetAllAsync();
        return faqs.Select(MapToDto).ToList();
    }

    public async Task<FAQDto?> GetByIdAsync(Guid id)
    {
        var faq = await _faqRepository.GetByIdAsync(id);
        return faq != null ? MapToDto(faq) : null;
    }

    public async Task<List<FAQSearchResult>> SearchAsync(string query, int limit = 5, double threshold = 0.3)
    {
        // Generate embedding for the query
        var embedding = await _embeddingService.GenerateEmbeddingAsync(query);
        
        // Search using semantic similarity
        var results = await _faqRepository.SearchByEmbeddingAsync(embedding, limit, threshold);
        
        return results.Select(r => new FAQSearchResult
        {
            FAQ = MapToDto(r.FAQ),
            Distance = r.Distance
        }).ToList();
    }

    public async Task<FAQDto> CreateAsync(CreateFAQDto dto)
    {
        // Generate embedding for the question
        var embedding = await _embeddingService.GenerateEmbeddingAsync(dto.Question);
        
        var faq = new FAQ
        {
            Question = dto.Question,
            Answer = dto.Answer,
            Category = dto.Category,
            Embedding = new Vector(embedding)
        };

        var created = await _faqRepository.CreateAsync(faq);
        return MapToDto(created);
    }

    public async Task<FAQDto> UpdateAsync(Guid id, UpdateFAQDto dto)
    {
        var faq = await _faqRepository.GetByIdAsync(id);
        if (faq == null)
        {
            throw new KeyNotFoundException($"FAQ with ID {id} not found");
        }

        // Update properties
        faq.Question = dto.Question;
        faq.Answer = dto.Answer;
        faq.Category = dto.Category;
        
        // Regenerate embedding if question changed
        var embedding = await _embeddingService.GenerateEmbeddingAsync(dto.Question);
        faq.Embedding = new Vector(embedding);

        await _faqRepository.UpdateAsync(faq);
        return MapToDto(faq);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _faqRepository.DeleteAsync(id);
    }

    private static FAQDto MapToDto(FAQ faq)
    {
        return new FAQDto
        {
            Id = faq.Id,
            Question = faq.Question,
            Answer = faq.Answer,
            Category = faq.Category,
            CreatedAt = faq.CreatedAt,
            UpdatedAt = faq.UpdatedAt
        };
    }
}
