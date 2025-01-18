using Academy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class RequestLogConfiguration : IEntityTypeConfiguration<RequestLogEntity>
{
    public void Configure(EntityTypeBuilder<RequestLogEntity> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RequestId).IsRequired()
            .HasMaxLength(64);
        builder.Property(r => r.ClientIP).IsRequired()
            .HasMaxLength(45);
        builder.Property(r => r.UserAgent).IsRequired();
        builder.Property(r => r.Path).IsRequired()
            .HasMaxLength(2048);
        builder.Property(r => r.Method).IsRequired()
            .HasMaxLength(10);
        builder.Property(r => r.RequestDate).IsRequired();
        builder.Property(r => r.Ticks).IsRequired();
        builder.Property(r => r.StatusCode).IsRequired();
        builder.Property(r => r.Message).IsRequired()
            .HasMaxLength(1000);
        builder.Property(r => r.ExceptionType)
            .HasMaxLength(100);
        builder.Property(r => r.ErrorLocation);
        
        builder.HasIndex(r => r.RequestId).IsUnique();
        builder.HasIndex(r => r.ClientIP);
        builder.HasIndex(r => r.RequestDate);
    }
}