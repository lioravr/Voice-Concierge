using FluentAssertions;
using Moq;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.Services;
using Xunit;

namespace VoiceConcierge.Tests.Infrastructure.Services;

public class UnansweredQuestionServiceTests
{
    private readonly Mock<IUnansweredQuestionRepository> _repositoryMock;
    private readonly UnansweredQuestionService _service;

    public UnansweredQuestionServiceTests()
    {
        _repositoryMock = new Mock<IUnansweredQuestionRepository>();
        _service = new UnansweredQuestionService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Questions()
    {
        // Arrange
        var questions = new List<UnansweredQuestion>
        {
            new UnansweredQuestion { Id = 1, Question = "Q1", IsResolved = false },
            new UnansweredQuestion { Id = 2, Question = "Q2", IsResolved = true }
        };
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(questions);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(questions);
    }

    [Fact]
    public async Task GetPendingAsync_Should_Return_Only_Unresolved_Questions()
    {
        // Arrange
        var questions = new List<UnansweredQuestion>
        {
            new UnansweredQuestion { Id = 1, Question = "Q1", IsResolved = false },
            new UnansweredQuestion { Id = 2, Question = "Q2", IsResolved = false }
        };
        _repositoryMock.Setup(x => x.GetPendingAsync()).ReturnsAsync(questions);

        // Act
        var result = await _service.GetPendingAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(q => !q.IsResolved);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_New_Question()
    {
        // Arrange
        var question = new UnansweredQuestion 
        { 
            Question = "New question", 
            SessionId = "session-123" 
        };
        
        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<UnansweredQuestion>()))
            .ReturnsAsync((UnansweredQuestion q) => { q.Id = 1; return q; });

        // Act
        var result = await _service.CreateAsync(question);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<UnansweredQuestion>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsResolvedAsync_Should_Mark_Question_Resolved()
    {
        // Arrange
        var question = new UnansweredQuestion 
        { 
            Id = 1, 
            Question = "Test", 
            IsResolved = false 
        };
        
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(question);
        _repositoryMock.Setup(x => x.UpdateAsync(1, It.IsAny<UnansweredQuestion>()))
            .ReturnsAsync((int id, UnansweredQuestion q) => { q.IsResolved = true; return q; });

        // Act
        var result = await _service.MarkAsResolvedAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.IsResolved.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Question()
    {
        // Arrange
        _repositoryMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
    }
}
