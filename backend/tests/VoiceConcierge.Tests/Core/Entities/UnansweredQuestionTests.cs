using FluentAssertions;
using VoiceConcierge.Core.Domain.Entities;
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
            Frequency = 5,
            Status = "pending",
            FirstAskedAt = DateTime.UtcNow,
            LastAskedAt = DateTime.UtcNow
        };

        // Assert
        question.Question.Should().Be("Do you have a spa?");
        question.Frequency.Should().Be(5);
        question.Status.Should().Be("pending");
        question.FirstAskedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        question.LastAskedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UnansweredQuestion_Status_Can_Be_Changed()
    {
        // Arrange
        var question = new UnansweredQuestion
        {
            Question = "Test question",
            Status = "pending"
        };

        // Act
        question.Status = "converted";

        // Assert
        question.Status.Should().Be("converted");
    }

    [Fact]
    public void UnansweredQuestion_Frequency_Should_Default_To_One()
    {
        // Arrange & Act
        var question = new UnansweredQuestion { Question = "Test" };

        // Assert
        question.Frequency.Should().Be(1);
    }

    [Fact]
    public void UnansweredQuestion_Should_Have_Guid_Id()
    {
        // Arrange & Act
        var question = new UnansweredQuestion { Id = Guid.NewGuid() };

        // Assert
        question.Id.Should().NotBe(Guid.Empty);
    }
}
