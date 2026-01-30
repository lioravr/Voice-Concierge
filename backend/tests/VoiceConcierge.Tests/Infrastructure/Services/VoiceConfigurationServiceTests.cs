using FluentAssertions;
using Moq;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.Services;
using Xunit;
using Microsoft.Extensions.Configuration;
using Moq.Protected;
using System.Net;

namespace VoiceConcierge.Tests.Infrastructure.Services;

public class VoiceConfigurationServiceTests
{
    private readonly Mock<IVoiceConfigurationRepository> _repositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly VoiceConfigurationService _voiceService;

    public VoiceConfigurationServiceTests()
    {
        _repositoryMock = new Mock<IVoiceConfigurationRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        // Setup HTTP client factory
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Setup configuration
        _configurationMock
            .Setup(x => x["OpenAI:ApiKey"])
            .Returns("test-api-key");

        _voiceService = new VoiceConfigurationService(
            _repositoryMock.Object,
            _configurationMock.Object,
            _httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Voices()
    {
        // Arrange
        var voices = new List<VoiceConfiguration>
        {
            new VoiceConfiguration 
            { 
                Id = Guid.NewGuid(), 
                VoiceId = 1, 
                Name = "James", 
                Description = "Professional male voice",
                ProviderVoiceId = "alloy",
                IsActive = true 
            },
            new VoiceConfiguration 
            { 
                Id = Guid.NewGuid(), 
                VoiceId = 2, 
                Name = "Sofia", 
                Description = "Friendly female voice",
                ProviderVoiceId = "nova",
                IsActive = false 
            }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(voices);

        // Act
        var result = await _voiceService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("James");
        result[0].VoiceId.Should().Be(1);
        result[0].IsActive.Should().BeTrue();
        result[1].Name.Should().Be("Sofia");
        result[1].VoiceId.Should().Be(2);
        result[1].IsActive.Should().BeFalse();
        _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetActiveAsync_Should_Return_Active_Voice()
    {
        // Arrange
        var activeVoice = new VoiceConfiguration
        {
            Id = Guid.NewGuid(),
            VoiceId = 1,
            Name = "James",
            Description = "Professional male voice",
            ProviderVoiceId = "alloy",
            IsActive = true
        };

        _repositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync(activeVoice);

        // Act
        var result = await _voiceService.GetActiveAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("James");
        result.IsActive.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetActiveAsync(), Times.Once);
    }

    [Fact]
    public async Task GetActiveAsync_Should_Return_Null_When_No_Active_Voice()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.GetActiveAsync())
            .ReturnsAsync((VoiceConfiguration?)null);

        // Act
        var result = await _voiceService.GetActiveAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByVoiceIdAsync_Should_Return_Voice_When_Found()
    {
        // Arrange
        var voiceId = 2;
        var voice = new VoiceConfiguration
        {
            Id = Guid.NewGuid(),
            VoiceId = voiceId,
            Name = "Sofia",
            Description = "Friendly female voice",
            ProviderVoiceId = "nova",
            IsActive = false
        };

        _repositoryMock
            .Setup(x => x.GetByVoiceIdAsync(voiceId))
            .ReturnsAsync(voice);

        // Act
        var result = await _voiceService.GetByVoiceIdAsync(voiceId);

        // Assert
        result.Should().NotBeNull();
        result!.VoiceId.Should().Be(voiceId);
        result.Name.Should().Be("Sofia");
        _repositoryMock.Verify(x => x.GetByVoiceIdAsync(voiceId), Times.Once);
    }

    [Fact]
    public async Task GetByVoiceIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var voiceId = 999;

        _repositoryMock
            .Setup(x => x.GetByVoiceIdAsync(voiceId))
            .ReturnsAsync((VoiceConfiguration?)null);

        // Act
        var result = await _voiceService.GetByVoiceIdAsync(voiceId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetActiveAsync_Should_Call_Repository()
    {
        // Arrange
        var voiceId = 3;

        _repositoryMock
            .Setup(x => x.SetActiveAsync(voiceId))
            .Returns(Task.CompletedTask);

        // Act
        await _voiceService.SetActiveAsync(voiceId);

        // Assert
        _repositoryMock.Verify(x => x.SetActiveAsync(voiceId), Times.Once);
    }

    [Fact]
    public async Task GeneratePreviewAsync_Should_Return_Null_When_Voice_Not_Found()
    {
        // Arrange
        var voiceId = 999;

        _repositoryMock
            .Setup(x => x.GetByVoiceIdAsync(voiceId))
            .ReturnsAsync((VoiceConfiguration?)null);

        // Act
        var result = await _voiceService.GeneratePreviewAsync(voiceId);

        // Assert
        result.Should().BeNull();
        _repositoryMock.Verify(x => x.GetByVoiceIdAsync(voiceId), Times.Once);
    }

    [Fact]
    public async Task GeneratePreviewAsync_Should_Call_OpenAI_TTS_API()
    {
        // Arrange
        var voiceId = 1;
        var voice = new VoiceConfiguration
        {
            Id = Guid.NewGuid(),
            VoiceId = voiceId,
            Name = "James",
            ProviderVoiceId = "alloy",
            IsActive = true
        };
        var audioData = new byte[] { 1, 2, 3, 4, 5 };

        _repositoryMock
            .Setup(x => x.GetByVoiceIdAsync(voiceId))
            .ReturnsAsync(voice);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(audioData)
            });

        // Act
        var result = await _voiceService.GeneratePreviewAsync(voiceId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(audioData);
        _repositoryMock.Verify(x => x.GetByVoiceIdAsync(voiceId), Times.Once);
        
        // Verify HTTP request was made
        _httpMessageHandlerMock
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString() == "https://api.openai.com/v1/audio/speech"),
                ItExpr.IsAny<CancellationToken>());
    }
}
