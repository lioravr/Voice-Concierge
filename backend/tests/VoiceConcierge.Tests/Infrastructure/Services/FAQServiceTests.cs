using FluentAssertions;
using Moq;
using Pgvector;
using VoiceConcierge.Core.Entities;
using VoiceConcierge.Core.Interfaces;
using VoiceConcierge.Infrastructure.Services;
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
            new FAQ { Id = 1, Question = "Q1", Answer = "A1", Category = "Cat1" },
            new FAQ { Id = 2, Question = "Q2", Answer = "A2", Category = "Cat2" }
        };
        _faqRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(faqs);

        // Act
        var result = await _faqService.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(faqs);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_FAQ_When_Exists()
    {
        // Arrange
        var faq = new FAQ { Id = 1, Question = "Test", Answer = "Answer" };
        _faqRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(faq);

        // Act
        var result = await _faqService.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(faq);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        _faqRepositoryMock.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((FAQ?)null);

        // Act
        var result = await _faqService.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Should_Generate_Embedding_And_Create_FAQ()
    {
        // Arrange
        var faq = new FAQ { Question = "New question?", Answer = "New answer" };
        var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });
        
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync("New question?"))
            .ReturnsAsync(embedding);
        
        _faqRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<FAQ>()))
            .ReturnsAsync((FAQ f) => { f.Id = 1; return f; });

        // Act
        var result = await _faqService.CreateAsync(faq);

        // Assert
        result.Should().NotBeNull();
        result.Embedding.Should().NotBeNull();
        result.Embedding.Should().Be(embedding);
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync("New question?"), Times.Once);
        _faqRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<FAQ>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Call_Repository_Delete()
    {
        // Arrange
        _faqRepositoryMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _faqService.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _faqRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_Should_Generate_Query_Embedding_And_Search()
    {
        // Arrange
        var query = "What time is check-in?";
        var embedding = new Vector(new float[] { 0.1f, 0.2f });
        var results = new List<FAQ>
        {
            new FAQ { Id = 1, Question = "Check-in time?", Answer = "3 PM" }
        };

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(query))
            .ReturnsAsync(embedding);
        
        _faqRepositoryMock
            .Setup(x => x.SearchByVectorAsync(embedding, 5))
            .ReturnsAsync(results);

        // Act
        var result = await _faqService.SearchAsync(query, 5);

        // Assert
        result.Should().HaveCount(1);
        result.First().Question.Should().Be("Check-in time?");
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync(query), Times.Once);
        _faqRepositoryMock.Verify(x => x.SearchByVectorAsync(embedding, 5), Times.Once);
    }

    [Fact]
    public async Task GetByCategoryAsync_Should_Return_FAQs_For_Category()
    {
        // Arrange
        var category = "Hotel Services";
        var faqs = new List<FAQ>
        {
            new FAQ { Id = 1, Question = "Q1", Category = category },
            new FAQ { Id = 2, Question = "Q2", Category = category }
        };
        _faqRepositoryMock.Setup(x => x.GetByCategoryAsync(category)).ReturnsAsync(faqs);

        // Act
        var result = await _faqService.GetByCategoryAsync(category);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(f => f.Category == category);
    }
}
