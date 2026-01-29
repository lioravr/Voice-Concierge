using FluentAssertions;
using Pgvector;
using VoiceConcierge.Core.Entities;
using Xunit;

namespace VoiceConcierge.Tests.Core.Entities;

public class FAQTests
{
    [Fact]
    public void FAQ_Should_Initialize_With_Valid_Properties()
    {
        // Arrange & Act
        var faq = new FAQ
        {
            Question = "What are your check-in hours?",
            Answer = "Check-in is from 3 PM to 11 PM.",
            Category = "Hotel Services",
            IsActive = true,
            Priority = 1,
            Embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f })
        };

        // Assert
        faq.Question.Should().Be("What are your check-in hours?");
        faq.Answer.Should().Be("Check-in is from 3 PM to 11 PM.");
        faq.Category.Should().Be("Hotel Services");
        faq.IsActive.Should().BeTrue();
        faq.Priority.Should().Be(1);
        faq.Embedding.Should().NotBeNull();
    }

    [Fact]
    public void FAQ_CreatedAt_Should_Be_Set()
    {
        // Arrange & Act
        var faq = new FAQ();
        var now = DateTime.UtcNow;

        // Assert
        faq.CreatedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void FAQ_Should_AllowEmpty_Question_For_Validation_Testing(string question)
    {
        // Arrange & Act
        var faq = new FAQ { Question = question };

        // Assert
        faq.Question.Should().Be(question);
    }
}
