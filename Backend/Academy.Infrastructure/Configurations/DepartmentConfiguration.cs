using Academy.Domain.Entities;
using Academy.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentEntity>
{
    public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(Department.MAX_NAME_LENGTH);
        builder.Property(d => d.Description)
            .HasMaxLength(Department.MAX_DESCRIPTION_LENGTH);
        builder.Property(d => d.Head)
            .IsRequired()
            .HasMaxLength(Department.MAX_HEAD_NAME_LENGTH);
        builder.Property(d => d.EstablishedDate)
            .IsRequired();

        builder.HasMany(d => d.Teachers)
            .WithOne(t => t.Department)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        
        
    }
}