using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;

namespace VoiceConcierge.Infrastructure.Data.Repositories;

public class UnansweredQuestionRepository : IUnansweredQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public UnansweredQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UnansweredQuestion>> GetAllPendingAsync()
    {
        return await _context.UnansweredQuestions
            .Where(q => q.Status == "pending")
            .OrderByDescending(q => q.Frequency)
            .ThenByDescending(q => q.LastAskedAt)
            .ToListAsync();
    }

    public async Task<UnansweredQuestion?> GetByIdAsync(Guid id)
    {
        return await _context.UnansweredQuestions.FindAsync(id);
    }

    public async Task<UnansweredQuestion> RecordAsync(string question)
    {
        // Check if question already exists (case-insensitive)
        var existing = await _context.UnansweredQuestions
            .FirstOrDefaultAsync(q =>
                q.Question.ToLower() == question.ToLower() &&
                q.Status == "pending");

        if (existing != null)
        {
            // Increment frequency
            existing.Frequency++;
            existing.LastAskedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing;
        }

        // Create new record
        var unansweredQuestion = new UnansweredQuestion
        {
            Id = Guid.NewGuid(),
            Question = question,
            Frequency = 1,
            FirstAskedAt = DateTime.UtcNow,
            LastAskedAt = DateTime.UtcNow,
            Status = "pending"
        };

        _context.UnansweredQuestions.Add(unansweredQuestion);
        await _context.SaveChangesAsync();

        return unansweredQuestion;
    }

    public async Task ConvertToFAQAsync(Guid questionId, string answer)
    {
        var question = await _context.UnansweredQuestions.FindAsync(questionId);
        if (question == null)
        {
            throw new InvalidOperationException($"Unanswered question with ID {questionId} not found.");
        }

        // Create FAQ
        var faq = new FAQ
        {
            Id = Guid.NewGuid(),
            Question = question.Question,
            Answer = answer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.FAQs.Add(faq);

        // Mark question as converted
        question.Status = "converted";
        await _context.SaveChangesAsync();
    }

    public async Task DismissAsync(Guid questionId)
    {
        var question = await _context.UnansweredQuestions.FindAsync(questionId);
        if (question != null)
        {
            question.Status = "dismissed";
            await _context.SaveChangesAsync();
        }
    }
}
