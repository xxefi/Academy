using Academy.Domain.Entities;
using Academy.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(Group.MAX_NAME_LENGTH);

        builder.HasOne(g => g.Faculty)
            .WithMany(f => f.Groups)
            .HasForeignKey(g => g.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Teacher)
            .WithMany()
            .HasForeignKey(g => g.TeacherId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(g => g.Students)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Teachers)
            .WithMany(t => t.Groups)
            .UsingEntity(
                "TeacherGroups",
                l => l.HasOne(typeof(TeacherEntity)).WithMany().HasForeignKey("TeacherId"),
                r => r.HasOne(typeof(GroupEntity)).WithMany().HasForeignKey("GroupId")
            );
    }
}