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
            VoiceId = 1,
            Name = "James",
            Description = "Professional male voice",
            Gender = "Male",
            Accent = "British",
            ProviderVoiceId = "alloy",
            IsActive = true
        };

        // Assert
        voice.VoiceId.Should().Be(1);
        voice.Name.Should().Be("James");
        voice.Description.Should().Be("Professional male voice");
        voice.Gender.Should().Be("Male");
        voice.Accent.Should().Be("British");
        voice.ProviderVoiceId.Should().Be("alloy");
        voice.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VoiceConfiguration_CreatedAt_Should_Be_DefaultValue()
    {
        // Arrange & Act
        var voice = new VoiceConfiguration();

        // Assert
        voice.CreatedAt.Should().Be(default(DateTime));
    }

    [Fact]
    public void VoiceConfiguration_IsActive_Can_Be_Toggled()
    {
        // Arrange
        var voice = new VoiceConfiguration { IsActive = false };

        // Act
        voice.IsActive = true;

        // Assert
        voice.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VoiceConfiguration_Should_Have_Guid_Id()
    {
        // Arrange & Act
        var voice = new VoiceConfiguration { Id = Guid.NewGuid() };

        // Assert
        voice.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void VoiceConfiguration_Should_Accept_Valid_VoiceId(int voiceId)
    {
        // Arrange & Act
        var voice = new VoiceConfiguration { VoiceId = voiceId };

        // Assert
        voice.VoiceId.Should().Be(voiceId);
    }
}
