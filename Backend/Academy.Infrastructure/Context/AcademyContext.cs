using Academy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Academy.Infrastructure.Context;

public class AcademyContext : DbContext
{
    public AcademyContext(DbContextOptions<AcademyContext> options)
    : base(options)
    {
    }
    
    public DbSet<DepartmentEntity> Departments { get; set; }
    public DbSet<FacultyEntity> Faculties { get; set; }
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RequestLogEntity> RequestLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademyContext).Assembly);
}