using FluentAssertions;
using Moq;
using Pgvector;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Domain.Interfaces;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Services;
using Xunit;

namespace VoiceConcierge.Tests.Infrastructure.Services;

public class FAQServiceTests
{
    private readonly Mock<IFAQRepository> _faqRepositoryMock;
    private readonly Mock<IEmbeddingService> _embeddingServiceMock;
    private readonly FAQService _faqService;

    public FAQServiceTests()
    {
        _faqRepositoryMock = new Mock<IFAQRepository>();
        _embeddingServiceMock = new Mock<IEmbeddingService>();
        _faqService = new FAQService(_faqRepositoryMock.Object, _embeddingServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_FAQs()
    {
        // Arrange
        var faqs = new List<FAQ>
        {
            new FAQ { Id = Guid.NewGuid(), Question = "Q1", Answer = "A1", Category = "Cat1" },
            new FAQ { Id = Guid.NewGuid(), Question = "Q2", Answer = "A2", Category = "Cat2" }
        };

        _faqRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(faqs);

        // Act
        var result = await _faqService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Question.Should().Be("Q1");
        result[1].Question.Should().Be("Q2");
        _faqRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_FAQ_When_Found()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var faq = new FAQ { Id = faqId, Question = "Test question?", Answer = "Test answer", Category = "Test" };

        _faqRepositoryMock
            .Setup(x => x.GetByIdAsync(faqId))
            .ReturnsAsync(faq);

        // Act
        var result = await _faqService.GetByIdAsync(faqId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(faqId);
        result.Question.Should().Be("Test question?");
        _faqRepositoryMock.Verify(x => x.GetByIdAsync(faqId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
    {
        // Arrange
        var faqId = Guid.NewGuid();

        _faqRepositoryMock
            .Setup(x => x.GetByIdAsync(faqId))
            .ReturnsAsync((FAQ?)null);

        // Act
        var result = await _faqService.GetByIdAsync(faqId);

        // Assert
        result.Should().BeNull();
        _faqRepositoryMock.Verify(x => x.GetByIdAsync(faqId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Generate_Embedding_And_Create_FAQ()
    {
        // Arrange
        var createDto = new CreateFAQDto 
        { 
            Question = "New question?", 
            Answer = "New answer",
            Category = "Test"
        };
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync("New question?"))
            .ReturnsAsync(embedding);
        
        _faqRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FAQ>()))
            .ReturnsAsync((FAQ f) => { f.Id = Guid.NewGuid(); return f; });

        // Act
        var result = await _faqService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Question.Should().Be("New question?");
        result.Answer.Should().Be("New answer");
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync("New question?"), Times.Once);
        _faqRepositoryMock.Verify(x => x.CreateAsync(It.Is<FAQ>(f => 
            f.Question == "New question?" && 
            f.Answer == "New answer" &&
            f.Embedding != null)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Regenerate_Embedding_And_Update_FAQ()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var existingFaq = new FAQ 
        { 
            Id = faqId, 
            Question = "Old question?", 
            Answer = "Old answer",
            Category = "Old"
        };
        var updateDto = new UpdateFAQDto
        {
            Question = "Updated question?",
            Answer = "Updated answer",
            Category = "Updated"
        };
        var embedding = new float[] { 0.4f, 0.5f, 0.6f };

        _faqRepositoryMock
            .Setup(x => x.GetByIdAsync(faqId))
            .ReturnsAsync(existingFaq);
        
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync("Updated question?"))
            .ReturnsAsync(embedding);
        
        _faqRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<FAQ>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _faqService.UpdateAsync(faqId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Question.Should().Be("Updated question?");
        result.Answer.Should().Be("Updated answer");
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync("Updated question?"), Times.Once);
        _faqRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<FAQ>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_FAQ_Not_Found()
    {
        // Arrange
        var faqId = Guid.NewGuid();
        var updateDto = new UpdateFAQDto { Question = "Test", Answer = "Test", Category = "Test" };

        _faqRepositoryMock
            .Setup(x => x.GetByIdAsync(faqId))
            .ReturnsAsync((FAQ?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _faqService.UpdateAsync(faqId, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository()
    {
        // Arrange
        var faqId = Guid.NewGuid();

        _faqRepositoryMock
            .Setup(x => x.DeleteAsync(faqId))
            .Returns(Task.CompletedTask);

        // Act
        await _faqService.DeleteAsync(faqId);

        // Assert
        _faqRepositoryMock.Verify(x => x.DeleteAsync(faqId), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_Should_Generate_Embedding_And_Search()
    {
        // Arrange
        var query = "check-in time";
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };
        var faq1 = new FAQ { Id = Guid.NewGuid(), Question = "What are check-in hours?", Answer = "Check-in is from 3 PM." };
        var faq2 = new FAQ { Id = Guid.NewGuid(), Question = "When can I check in?", Answer = "Check-in starts at 3 PM." };
        var searchResults = new List<(FAQ FAQ, double Distance)>
        {
            (faq1, 0.15),
            (faq2, 0.25)
        };

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(query))
            .ReturnsAsync(embedding);

        _faqRepositoryMock
            .Setup(x => x.SearchByEmbeddingAsync(embedding, 5, 0.3))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _faqService.SearchAsync(query, 5, 0.3);

        // Assert
        result.Should().HaveCount(2);
        result[0].FAQ.Question.Should().Be("What are check-in hours?");
        result[0].Distance.Should().Be(0.15);
        result[1].FAQ.Question.Should().Be("When can I check in?");
        result[1].Distance.Should().Be(0.25);
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync(query), Times.Once);
        _faqRepositoryMock.Verify(x => x.SearchByEmbeddingAsync(embedding, 5, 0.3), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_Should_Return_Empty_When_No_Results()
    {
        // Arrange
        var query = "something unknown";
        var embedding = new float[] { 0.7f, 0.8f, 0.9f };
        var searchResults = new List<(FAQ FAQ, double Distance)>();

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(query))
            .ReturnsAsync(embedding);

        _faqRepositoryMock
            .Setup(x => x.SearchByEmbeddingAsync(embedding, 5, 0.3))
            .ReturnsAsync(searchResults);

        // Act
        var result = await _faqService.SearchAsync(query, 5, 0.3);

        // Assert
        result.Should().BeEmpty();
    }
}
