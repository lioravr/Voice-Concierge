using FluentAssertions;
using Moq;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Services;
using Xunit;

namespace VoiceConcierge.Tests.Infrastructure.Services;

public class UnansweredQuestionServiceTests
{
    private readonly Mock<IUnansweredQuestionRepository> _questionRepositoryMock;
    private readonly Mock<IFAQService> _faqServiceMock;
    private readonly UnansweredQuestionService _questionService;

    public UnansweredQuestionServiceTests()
    {
        _questionRepositoryMock = new Mock<IUnansweredQuestionRepository>();
        _faqServiceMock = new Mock<IFAQService>();
        _questionService = new UnansweredQuestionService(
            _questionRepositoryMock.Object,
            _faqServiceMock.Object);
    }

    [Fact]
    public async Task GetAllPendingAsync_Should_Return_All_Pending_Questions()
    {
        // Arrange
        var questions = new List<UnansweredQuestion>
        {
            new UnansweredQuestion { Id = Guid.NewGuid(), Question = "Question 1", Status = "pending", Frequency = 1 },
            new UnansweredQuestion { Id = Guid.NewGuid(), Question = "Question 2", Status = "pending", Frequency = 3 }
        };

        _questionRepositoryMock
            .Setup(x => x.GetAllPendingAsync())
            .ReturnsAsync(questions);

        // Act
        var result = await _questionService.GetAllPendingAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Question.Should().Be("Question 1");
        result[1].Question.Should().Be("Question 2");
        result[1].Frequency.Should().Be(3);
        _questionRepositoryMock.Verify(x => x.GetAllPendingAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Question_When_Found()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var question = new UnansweredQuestion 
        { 
            Id = questionId, 
            Question = "Test question?", 
            Status = "pending",
            Frequency = 5 
        };

        _questionRepositoryMock
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync(question);

        // Act
        var result = await _questionService.GetByIdAsync(questionId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(questionId);
        result.Question.Should().Be("Test question?");
        result.Frequency.Should().Be(5);
        _questionRepositoryMock.Verify(x => x.GetByIdAsync(questionId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var questionId = Guid.NewGuid();

        _questionRepositoryMock
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync((UnansweredQuestion?)null);

        // Act
        var result = await _questionService.GetByIdAsync(questionId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RecordAsync_Should_Record_New_Question()
    {
        // Arrange
        var questionText = "Do you have a spa?";
        var recordedQuestion = new UnansweredQuestion
        {
            Id = Guid.NewGuid(),
            Question = questionText,
            Status = "pending",
            Frequency = 1,
            FirstAskedAt = DateTime.UtcNow,
            LastAskedAt = DateTime.UtcNow
        };

        _questionRepositoryMock
            .Setup(x => x.RecordAsync(questionText))
            .ReturnsAsync(recordedQuestion);

        // Act
        var result = await _questionService.RecordAsync(questionText);

        // Assert
        result.Should().NotBeNull();
        result.Question.Should().Be(questionText);
        result.Status.Should().Be("pending");
        result.Frequency.Should().Be(1);
        _questionRepositoryMock.Verify(x => x.RecordAsync(questionText), Times.Once);
    }

    [Fact]
    public async Task ConvertToFAQAsync_Should_Create_FAQ_And_Mark_Question_Converted()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var question = new UnansweredQuestion
        {
            Id = questionId,
            Question = "Do you have a spa?",
            Status = "pending",
            Frequency = 5
        };
        var answer = "Yes, we have a luxurious spa on the 3rd floor.";
        var category = "Amenities";
        var createdFaq = new FAQDto
        {
            Id = Guid.NewGuid(),
            Question = question.Question,
            Answer = answer,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        _questionRepositoryMock
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync(question);

        _faqServiceMock
            .Setup(x => x.CreateAsync(It.Is<CreateFAQDto>(dto => 
                dto.Question == question.Question && 
                dto.Answer == answer &&
                dto.Category == category)))
            .ReturnsAsync(createdFaq);

        _questionRepositoryMock
            .Setup(x => x.ConvertToFAQAsync(questionId, answer))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _questionService.ConvertToFAQAsync(questionId, answer, category);

        // Assert
        result.Should().NotBeNull();
        result.Question.Should().Be(question.Question);
        result.Answer.Should().Be(answer);
        result.Category.Should().Be(category);
        _faqServiceMock.Verify(x => x.CreateAsync(It.IsAny<CreateFAQDto>()), Times.Once);
        _questionRepositoryMock.Verify(x => x.ConvertToFAQAsync(questionId, answer), Times.Once);
    }

    [Fact]
    public async Task ConvertToFAQAsync_Should_Throw_When_Question_Not_Found()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var answer = "Some answer";

        _questionRepositoryMock
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync((UnansweredQuestion?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _questionService.ConvertToFAQAsync(questionId, answer));
    }

    [Fact]
    public async Task DismissAsync_Should_Call_Repository()
    {
        // Arrange
        var questionId = Guid.NewGuid();

        _questionRepositoryMock
            .Setup(x => x.DismissAsync(questionId))
            .Returns(Task.CompletedTask);

        // Act
        await _questionService.DismissAsync(questionId);

        // Assert
        _questionRepositoryMock.Verify(x => x.DismissAsync(questionId), Times.Once);
    }
}
