using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector.EntityFrameworkCore;
using VoiceConcierge.Core.Domain.Entities;

namespace VoiceConcierge.Infrastructure.Data.Configurations;

public class FAQConfiguration : IEntityTypeConfiguration<FAQ>
{
    public void Configure(EntityTypeBuilder<FAQ> builder)
    {
        builder.ToTable("faqs");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Question)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(f => f.Answer)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(f => f.Category)
            .HasColumnType("text");

        // Configure vector embedding column with dimension 1536 (OpenAI text-embedding-3-small)
        builder.Property(f => f.Embedding)
            .HasColumnType("vector(1536)");

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(f => f.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Create IVFFlat index for vector similarity search (cosine distance)
        builder.HasIndex(f => f.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
    }
}
