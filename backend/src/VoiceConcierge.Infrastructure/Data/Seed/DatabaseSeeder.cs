using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pgvector;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.Infrastructure.Data.Seed;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ApplicationDbContext context,
        IEmbeddingService embeddingService,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Starting database seeding...");

        try
        {
            // Always seed users (if they don't exist)
            await SeedUsersAsync();

            // Check if other data already exists
            if (await _context.FAQs.AnyAsync() || await _context.VoiceConfigurations.AnyAsync())
            {
                _logger.LogInformation("Database already contains FAQ/Voice data. Skipping seeding.");
                return;
            }

            await SeedVoiceConfigurationsAsync();
            await SeedFAQsAsync();

            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding database");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users already exist. Skipping user seeding.");
            return;
        }

        _logger.LogInformation("Seeding default users...");

        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = HashPassword("admin123"),
                Role = UserRoles.Admin,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "guest",
                PasswordHash = HashPassword("guest123"),
                Role = UserRoles.Guest,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} users", users.Count);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private async Task SeedVoiceConfigurationsAsync()
    {
        _logger.LogInformation("Seeding voice configurations from JSON...");

        var voiceData = MeridianSeedData.VoiceConfigurations.Voices;
        var voices = voiceData.Select(v =>
            new VoiceConfiguration
            {
                Id = Guid.NewGuid(),
                VoiceId = v.VoiceId,
                Name = v.Name,
                Description = v.Description,
                Gender = v.Gender,
                Accent = v.Accent,
                ProviderVoiceId = v.ProviderVoiceId,
                IsActive = v.VoiceId == 1, // Set James (voice 1) as default active
                CreatedAt = DateTime.UtcNow
            }).ToList();

        await _context.VoiceConfigurations.AddRangeAsync(voices);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} voice configurations", voices.Count);
    }

    private async Task SeedFAQsAsync()
    {
        _logger.LogInformation("Seeding FAQs from JSON with embeddings... This may take a few minutes.");

        var faqData = MeridianSeedData.FAQs.Items;
        var faqs = new List<FAQ>();
        var totalFaqs = faqData.Count;
        var processedCount = 0;

        foreach (var item in faqData)
        {
            processedCount++;
            _logger.LogInformation("Generating embedding for FAQ {Current}/{Total}: {Question}",
                processedCount, totalFaqs, item.Question);

            try
            {
                // Generate embedding for the question
                var embedding = await _embeddingService.GenerateEmbeddingAsync(item.Question);

                var faq = new FAQ
                {
                    Id = Guid.NewGuid(),
                    Question = item.Question,
                    Answer = item.Answer,
                    Category = item.Category,
                    Embedding = new Vector(embedding),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                faqs.Add(faq);

                // Add small delay to avoid rate limiting
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding for FAQ: {Question}", item.Question);
                throw;
            }
        }

        await _context.FAQs.AddRangeAsync(faqs);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} FAQs with embeddings", faqs.Count);
    }

    /// <summary>
    /// Force reseeds the database by clearing existing data
    /// WARNING: This will delete all existing FAQs and Voice Configurations
    /// </summary>
    public async Task ReseedAsync()
    {
        _logger.LogWarning("Force reseeding database - clearing existing data...");

        // Clear existing data
        _context.FAQs.RemoveRange(_context.FAQs);
        _context.VoiceConfigurations.RemoveRange(_context.VoiceConfigurations);
        await _context.SaveChangesAsync();

        // Reseed
        await SeedAsync();
    }
}
