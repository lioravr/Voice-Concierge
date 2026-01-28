using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;

namespace VoiceConcierge.Core.Services;

public class UnansweredQuestionService : IUnansweredQuestionService
{
    private readonly IUnansweredQuestionRepository _questionRepository;
    private readonly IFAQService _faqService;

    public UnansweredQuestionService(
        IUnansweredQuestionRepository questionRepository,
        IFAQService faqService)
    {
        _questionRepository = questionRepository;
        _faqService = faqService;
    }

    public async Task<List<UnansweredQuestionDto>> GetAllPendingAsync()
    {
        var questions = await _questionRepository.GetAllPendingAsync();
        return questions.Select(MapToDto).ToList();
    }

    public async Task<UnansweredQuestionDto?> GetByIdAsync(Guid id)
    {
        var question = await _questionRepository.GetByIdAsync(id);
        return question != null ? MapToDto(question) : null;
    }

    public async Task<UnansweredQuestionDto> RecordAsync(string question)
    {
        var recorded = await _questionRepository.RecordAsync(question);
        return MapToDto(recorded);
    }

    public async Task<FAQDto> ConvertToFAQAsync(Guid questionId, string answer, string? category = null)
    {
        // Get the question
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question == null)
        {
            throw new KeyNotFoundException($"Unanswered question with ID {questionId} not found");
        }

        // Create FAQ using FAQ service (which will generate embeddings)
        var createDto = new CreateFAQDto
        {
            Question = question.Question,
            Answer = answer,
            Category = category
        };
        
        var faq = await _faqService.CreateAsync(createDto);
        
        // Mark question as converted
        await _questionRepository.ConvertToFAQAsync(questionId, answer);
        
        return faq;
    }

    public async Task DismissAsync(Guid questionId)
    {
        await _questionRepository.DismissAsync(questionId);
    }

    private static UnansweredQuestionDto MapToDto(UnansweredQuestion question)
    {
        return new UnansweredQuestionDto
        {
            Id = question.Id,
            Question = question.Question,
            Frequency = question.Frequency,
            FirstAskedAt = question.FirstAskedAt,
            LastAskedAt = question.LastAskedAt,
            Status = question.Status
        };
    }
}
