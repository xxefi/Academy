using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AcademyContext _context;

    public DepartmentRepository(AcademyContext context)
        => _context = context;

    private Department CreateDepartment(DepartmentEntity entity)
    {
        var (department, departmentErrors) = Department.Create(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.Head,
            entity.EstablishedDate,
            entity.FacultyId,
            [],
            []
        );

        if (!string.IsNullOrEmpty(departmentErrors))
            throw new AcademyException(ExceptionType.InvalidRequest, departmentErrors);

        return department!;
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Departments
            .AsNoTracking()
            .Include(d => d.Faculty)
            .Include(d => d.Teachers)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new AcademyException(ExceptionType.NotFound, "DepartmentNotFound");

        return CreateDepartment(entity);
    }

    public async Task<ICollection<Department>> GetAllAsync()
    {
        var entities = await _context.Departments
            .AsNoTracking()
            .Include(d => d.Faculty)
            .Include(d => d.Teachers)
            .ToListAsync();

        if (!entities.Any())
            throw new AcademyException(ExceptionType.NotFound, "NoDepartmentsFound");

        return entities
            .Select(CreateDepartment)
            .ToList();
    }

    public async Task AddAsync(Department department)
    {
        var entity = new DepartmentEntity
        {
            Name = department.Name,
            Description = department.Description,
            Head = department.Head,
            EstablishedDate = department.EstablishedDate,
            FacultyId = department.FacultyId ?? Guid.Empty,
        };

        await _context.Departments
            .AddAsync(entity)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(IEnumerable<Department> departments)
    {
        foreach (var department in departments)
        {
            var entity = await _context.Departments.FindAsync(department.Id)
                ?? throw new AcademyException(ExceptionType.NotFound, "DepartmentNotFound");
            
            entity.Name = department.Name;
            entity.Description = department.Description;
            entity.Head = department.Head;
            entity.EstablishedDate = department.EstablishedDate;
            entity.FacultyId = department.FacultyId;

            _context.Departments.Update(entity);
        }
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Departments.FindAsync(id)
            ?? throw new AcademyException(ExceptionType.NotFound, "DepartmentNotFound");
        
        _context.Departments.Remove(entity);
    }

    public async Task<ICollection<Department>> FindAsync(Expression<Func<DepartmentEntity, bool>> predicate)
    {
        var entities = await _context.Departments
            .AsNoTracking()
            .Include(d => d.Faculty)
            .Include(d => d.Teachers)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any()) return new List<Department>();
        
        return entities
            .Select(CreateDepartment)
            .ToList();
    }
}
