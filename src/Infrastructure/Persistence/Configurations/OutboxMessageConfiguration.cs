using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OaigLoan.Domain.Entities;

namespace OaigLoan.Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Type).HasMaxLength(256).IsRequired();
        builder.Property(o => o.Content).HasColumnType("jsonb").IsRequired();
        builder.Property(o => o.OccurredOnUtc).HasColumnType("timestamp with time zone");
        builder.Property(o => o.ProcessedOnUtc).HasColumnType("timestamp with time zone");
        builder.Property(o => o.Error).HasMaxLength(2000);
    }
}
