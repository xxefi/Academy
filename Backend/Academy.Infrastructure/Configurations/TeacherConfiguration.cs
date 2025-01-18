using Academy.Domain.Entities;
using Academy.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<TeacherEntity>
{
    public void Configure(EntityTypeBuilder<TeacherEntity> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(Teacher.MAX_NAME_LENGTH);
        builder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(Teacher.MAX_NAME_LENGTH);
        
        builder.HasOne(t => t.Department)
            .WithMany(d => d.Teachers)
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(t => t.Groups)
            .WithMany(g => g.Teachers)
            .UsingEntity(
                "TeacherGroups",
                l => l.HasOne(typeof(GroupEntity)).WithMany().HasForeignKey("GroupId"),
                r => r.HasOne(typeof(TeacherEntity)).WithMany().HasForeignKey("TeacherId")
            );
    }
}