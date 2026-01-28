using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Core.Domain.Interfaces;

public interface IUnansweredQuestionRepository
{
    Task<List<UnansweredQuestion>> GetAllPendingAsync();
    
    Task<UnansweredQuestion?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Records a new unanswered question or increments frequency if it already exists
    /// </summary>
    Task<UnansweredQuestion> RecordAsync(string question);
    
    /// <summary>
    /// Converts an unanswered question to an FAQ
    /// </summary>
    Task ConvertToFAQAsync(Guid questionId, string answer);
    
    Task DismissAsync(Guid id);
}
