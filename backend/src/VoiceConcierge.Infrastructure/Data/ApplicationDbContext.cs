using Microsoft.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<UnansweredQuestion> UnansweredQuestions { get; set; }
    public DbSet<VoiceConfiguration> VoiceConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Enable pgvector extension
        modelBuilder.HasPostgresExtension("vector");
    }
}
