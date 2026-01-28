using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Infrastructure.Data.Configurations;

public class VoiceConfigurationConfiguration : IEntityTypeConfiguration<VoiceConfiguration>
{
    public void Configure(EntityTypeBuilder<VoiceConfiguration> builder)
    {
        builder.ToTable("voice_configurations");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VoiceId)
            .IsRequired();

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Description)
            .HasColumnType("text");

        builder.Property(v => v.Gender)
            .HasMaxLength(50);

        builder.Property(v => v.Accent)
            .HasMaxLength(50);

        builder.Property(v => v.ProviderVoiceId)
            .HasMaxLength(200);

        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Unique constraint on VoiceId
        builder.HasIndex(v => v.VoiceId)
            .IsUnique();

        // Index for quickly finding active voice
        builder.HasIndex(v => v.IsActive);
    }
}
