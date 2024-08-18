using Academy.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Data.Contexts;

public class AcademyContext : DbContext
{
    public AcademyContext()
    {
        
    }

    public AcademyContext(DbContextOptions<AcademyContext> options) : base(options)
    {
        
    }


    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Group> Groups{ get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Department>()
            .HasMany(t => t.Teachers)
            .WithOne(d => d.Department)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Faculty>()
            .HasMany(g => g.Groups)
            .WithOne(f => f.Faculty)
            .HasForeignKey(f => f.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Group>()
            .HasMany(s => s.Students)
            .WithOne(g => g.Groups)
            .HasForeignKey(g => g.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Teacher>()
            .HasMany(g => g.Groups)
            .WithOne(t => t.Teacher) 
            .HasForeignKey(t => t.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);


        var userEntity = modelBuilder.Entity<User>();

        userEntity.HasKey(u => u.Id);

        userEntity.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        userEntity.HasIndex(u => u.Username)
            .IsUnique();

        userEntity.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);
        userEntity.HasIndex(u => u.Email)
            .IsUnique();

        userEntity.Property(u => u.Password)
            .IsRequired();
    }
}
