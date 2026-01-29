using FluentAssertions;
using Moq;
using VoiceConcierge.Core.Entities;
using VoiceConcierge.Core.Interfaces;
using VoiceConcierge.Infrastructure.Services;
using Xunit;

namespace VoiceConcierge.Tests.Infrastructure.Services;

public class VoiceConfigurationServiceTests
{
    private readonly Mock<IVoiceConfigurationRepository> _repositoryMock;
    private readonly VoiceConfigurationService _service;

    public VoiceConfigurationServiceTests()
    {
        _repositoryMock = new Mock<IVoiceConfigurationRepository>();
        _service = new VoiceConfigurationService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Configurations()
    {
        // Arrange
        var configs = new List<VoiceConfiguration>
        {
            new VoiceConfiguration { Id = 1, Name = "James", IsActive = true },
            new VoiceConfiguration { Id = 2, Name = "Sofia", IsActive = false }
        };
        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(configs);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(configs);
    }

    [Fact]
    public async Task GetActiveAsync_Should_Return_Only_Active_Configuration()
    {
        // Arrange
        var activeConfig = new VoiceConfiguration 
        { 
            Id = 1, 
            Name = "James", 
            IsActive = true 
        };
        _repositoryMock.Setup(x => x.GetActiveAsync()).ReturnsAsync(activeConfig);

        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(activeConfig);
        result!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Configuration_When_Exists()
    {
        // Arrange
        var config = new VoiceConfiguration { Id = 1, Name = "James" };
        _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(config);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(config);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_New_Configuration()
    {
        // Arrange
        var config = new VoiceConfiguration 
        { 
            Name = "Marcus", 
            Provider = "OpenAI",
            ProviderVoiceId = "onyx"
        };
        
        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<VoiceConfiguration>()))
            .ReturnsAsync((VoiceConfiguration c) => { c.Id = 3; return c; });

        // Act
        var result = await _service.CreateAsync(config);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(3);
        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<VoiceConfiguration>()), Times.Once);
    }

    [Fact]
    public async Task SetActiveAsync_Should_Deactivate_Others_And_Activate_Target()
    {
        // Arrange
        var targetId = 2;
        var allConfigs = new List<VoiceConfiguration>
        {
            new VoiceConfiguration { Id = 1, Name = "James", IsActive = true },
            new VoiceConfiguration { Id = 2, Name = "Sofia", IsActive = false },
            new VoiceConfiguration { Id = 3, Name = "Marcus", IsActive = false }
        };

        _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(allConfigs);
        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<VoiceConfiguration>()))
            .ReturnsAsync((int id, VoiceConfiguration c) => c);

        // Act
        var result = await _service.SetActiveAsync(targetId);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<int>(), It.Is<VoiceConfiguration>(v => !v.IsActive)), 
            Times.Exactly(2)); // Deactivate 2 others
        _repositoryMock.Verify(
            x => x.UpdateAsync(targetId, It.Is<VoiceConfiguration>(v => v.IsActive)), 
            Times.Once); // Activate target
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Configuration()
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
