using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Infrastructure.Data.Configurations;

public class UnansweredQuestionConfiguration : IEntityTypeConfiguration<UnansweredQuestion>
{
    public void Configure(EntityTypeBuilder<UnansweredQuestion> builder)
    {
        builder.ToTable("unanswered_questions");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Question)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(u => u.Frequency)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(u => u.FirstAskedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.LastAskedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue("pending");

        // Indexes for common queries
        builder.HasIndex(u => u.Status);
        builder.HasIndex(u => u.Frequency);
    }
}
