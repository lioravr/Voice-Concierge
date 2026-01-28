using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public interface IFAQService
{
    Task<List<FAQDto>> GetAllAsync();
    Task<FAQDto?> GetByIdAsync(Guid id);
    Task<List<FAQSearchResult>> SearchAsync(string query, int limit = 5, double threshold = 0.3);
    Task<FAQDto> CreateAsync(CreateFAQDto dto);
    Task<FAQDto> UpdateAsync(Guid id, UpdateFAQDto dto);
    Task DeleteAsync(Guid id);
}
