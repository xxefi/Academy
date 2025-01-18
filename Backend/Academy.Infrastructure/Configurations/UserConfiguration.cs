using Academy.Domain.Entities;
using Academy.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(User.MAX_USERNAME_LENGTH);
        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(User.MAX_PASSWORD_LENGTH);
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(User.MAX_EMAIL_LENGTH);
        builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);
        builder.Property(u => u.RefreshTokenExpiryTime)
            .IsRequired(false);
        
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}