using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly AcademyContext _context;

    public StudentRepository(AcademyContext context)
        => _context = context;

    private Student CreateStudent(StudentEntity entity)
    {
        var (student, errors) = Student.Create(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.GroupId,
            null!
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);

        return student;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id)
            .ConfigureAwait(false)
            ?? throw new AcademyException(ExceptionType.NotFound, "StudentNotFound");

        return CreateStudent(entity);
    }

    public async Task<ICollection<Student>> GetAllAsync()
    {
        var entities = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
            .ToListAsync();

        return entities.Any()
            ? entities.Select(CreateStudent).ToList()
            : throw new AcademyException(ExceptionType.NotFound, "StudentNoStudents");
    }

    public async Task AddAsync(Student student)
    {
        var entity = new StudentEntity
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            GroupId = student.GroupId
        };

        await _context.Students
            .AddAsync(entity)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(Student student)
    {
        var entity = await _context.Students
                .FindAsync(student.Id)
                .ConfigureAwait(false)
            ?? throw new AcademyException(ExceptionType.NotFound, "StudentNotFound");
        
        entity.FirstName = student.FirstName;
        entity.LastName = student.LastName;
        entity.GroupId = student.GroupId;
        
        _context.Students.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Students
                  .FindAsync(id)
                  .ConfigureAwait(false)
            ?? throw new AcademyException(ExceptionType.NotFound, "StudentNotFound");
        
        _context.Students.Remove(entity);
    }

    public async Task<ICollection<Student>> FindAsync(Expression<Func<StudentEntity, bool>> predicate)
    {
        var entities = await _context.Students
            .AsNoTracking()
            .Include(s => s.Group)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any())
            return new List<Student>();

        var students = entities
            .Select(e => CreateStudent(e))
            .Where(s => s != null)
            .Cast<Student>()
            .ToList();

        return students;
    }
}
