using FluentAssertions;
using VoiceConcierge.Core.Entities;
using Xunit;

namespace VoiceConcierge.Tests.Core.Entities;

public class UnansweredQuestionTests
{
    [Fact]
    public void UnansweredQuestion_Should_Initialize_With_Valid_Properties()
    {
        // Arrange & Act
        var question = new UnansweredQuestion
        {
            Question = "Do you have a spa?",
            SessionId = "session-123",
            IsResolved = false
        };

        // Assert
        question.Question.Should().Be("Do you have a spa?");
        question.SessionId.Should().Be("session-123");
        question.IsResolved.Should().BeFalse();
        question.AskedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UnansweredQuestion_Can_Be_Resolved()
    {
        // Arrange
        var question = new UnansweredQuestion
        {
            Question = "Test question",
            IsResolved = false
        };

        // Act
        question.IsResolved = true;

        // Assert
        question.IsResolved.Should().BeTrue();
    }

    [Fact]
    public void UnansweredQuestion_Count_Should_Default_To_One()
    {
        // Arrange & Act
        var question = new UnansweredQuestion { Question = "Test" };

        // Assert
        question.Count.Should().Be(1);
    }
}
