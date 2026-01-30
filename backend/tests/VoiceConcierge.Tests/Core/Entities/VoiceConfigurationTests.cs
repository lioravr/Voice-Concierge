using FluentAssertions;
using VoiceConcierge.Core.Domain.Entities;
using Xunit;

namespace VoiceConcierge.Tests.Core.Entities;

public class VoiceConfigurationTests
{
    [Fact]
    public void VoiceConfiguration_Should_Initialize_With_Valid_Properties()
    {
        // Arrange & Act
        var voice = new VoiceConfiguration
        {
            Name = "James",
            Description = "Professional male voice",
            Provider = "OpenAI",
            ProviderVoiceId = "alloy",
            Language = "en-US",
            Speed = 1.0f,
            Pitch = 1.0f,
            IsActive = true
        };

        // Assert
        voice.Name.Should().Be("James");
        voice.Description.Should().Be("Professional male voice");
        voice.Provider.Should().Be("OpenAI");
        voice.ProviderVoiceId.Should().Be("alloy");
        voice.Language.Should().Be("en-US");
        voice.Speed.Should().Be(1.0f);
        voice.Pitch.Should().Be(1.0f);
        voice.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VoiceConfiguration_CreatedAt_Should_Be_Set()
    {
        // Arrange & Act
        var voice = new VoiceConfiguration();
        var now = DateTime.UtcNow;

        // Assert
        voice.CreatedAt.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    [InlineData(1.5f)]
    [InlineData(2.0f)]
    public void VoiceConfiguration_Speed_Should_Accept_Valid_Range(float speed)
    {
        // Arrange & Act
        var voice = new VoiceConfiguration { Speed = speed };

        // Assert
        voice.Speed.Should().Be(speed);
    }

    [Theory]
    [InlineData(0.5f)]
    [InlineData(1.0f)]
    [InlineData(1.5f)]
    [InlineData(2.0f)]
    public void VoiceConfiguration_Pitch_Should_Accept_Valid_Range(float pitch)
    {
        // Arrange & Act
        var voice = new VoiceConfiguration { Pitch = pitch };

        // Assert
        voice.Pitch.Should().Be(pitch);
    }
}
