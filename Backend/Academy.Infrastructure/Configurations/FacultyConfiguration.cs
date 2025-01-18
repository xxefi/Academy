using Academy.Domain.Entities;
using Academy.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<FacultyEntity>
{
    public void Configure(EntityTypeBuilder<FacultyEntity> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(Faculty.MAX_NAME_LENGTH);
        builder.Property(f => f.Description)
            .HasMaxLength(Faculty.MAX_DESCRIPTION_LENGTH);
        builder.Property(f => f.Dean)
            .IsRequired()
            .HasMaxLength(Faculty.MAX_DEAN_NAME_LENGTH);
        builder.Property(f => f.EstablishedDate)
            .IsRequired();

        builder.HasMany(f => f.Departments)
            .WithOne()
            .HasForeignKey(d => d.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Groups)
            .WithOne(g => g.Faculty)
            .HasForeignKey(g => g.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}