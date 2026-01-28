using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public interface IUnansweredQuestionService
{
    Task<List<UnansweredQuestionDto>> GetAllPendingAsync();
    Task<UnansweredQuestionDto?> GetByIdAsync(Guid id);
    Task<UnansweredQuestionDto> RecordAsync(string question);
    Task<FAQDto> ConvertToFAQAsync(Guid questionId, string answer, string? category = null);
    Task DismissAsync(Guid questionId);
}
