using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly AcademyContext _context;

    public TeacherRepository(AcademyContext context)
    {
        _context = context;
    }

    private Teacher? CreateTeacher(TeacherEntity entity)
    {
        var groups = entity.Groups.Select(g => {
            var (group, groupErrors) = Group.Create(
                g.Id,
                g.Name,
                g.FacultyId,
                null!,
                g.TeacherId,
                null!,
                [],
                []
            );

            if (!string.IsNullOrEmpty(groupErrors))
                throw new AcademyException(ExceptionType.InvalidRequest, groupErrors);

            return group!;
        }).ToList();

        var (teacher, errors) = Teacher.Create(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.DepartmentId ?? Guid.Empty,
            null!,
            groups
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);

        return teacher;
    }

    public async Task<Teacher?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Teachers
            .AsNoTracking()
            .Include(t => t.Department)
            .Include(t => t.Groups)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound");

        return CreateTeacher(entity);
    }

    public async Task<ICollection<Teacher>> GetAllAsync()
    {
        var entities = await _context.Teachers
            .AsNoTracking()
            .Include(t => t.Department)
            .Include(t => t.Groups)
            .ToListAsync();

        if (!entities.Any())
            throw new AcademyException(ExceptionType.NotFound, "TeacherNoTeachers");

        var teachers = entities
            .Select(e => CreateTeacher(e))
            .Where(t => t != null)
            .Cast<Teacher>()
            .ToList();

        return teachers;
    }

    public async Task AddAsync(Teacher teacher)
    {
        var entity = new TeacherEntity
        {
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            DepartmentId = teacher.DepartmentId
        };

        await _context.Teachers.AddAsync(entity);
    }

    public async Task UpdateAsync(Teacher teacher)
    {
        var entity = await _context.Teachers.FindAsync(teacher.Id)
            ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound");
        
        entity.FirstName = teacher.FirstName;
        entity.LastName = teacher.LastName;
        entity.DepartmentId = teacher.DepartmentId;
        
        _context.Teachers.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Teachers.FindAsync(id)
            ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound");
        
        _context.Teachers.Remove(entity);
    }

    public async Task<ICollection<Teacher>> FindAsync(Expression<Func<TeacherEntity, bool>> predicate)
    {
        var entities = await _context.Teachers
            .AsNoTracking()
            .Include(t => t.Department)
            .Include(t => t.Groups)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any())
            return new List<Teacher>();

        var teachers = entities
            .Select(e => CreateTeacher(e))
            .Where(t => t != null)
            .Cast<Teacher>()
            .ToList();

        return teachers;
    }
}
