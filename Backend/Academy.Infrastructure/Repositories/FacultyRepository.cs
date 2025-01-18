using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class FacultyRepository : IFacultyRepository
{
    private readonly AcademyContext _context;

    public FacultyRepository(AcademyContext context)
        => _context = context;

    private Faculty CreateFaculty(FacultyEntity entity)
    {
        var (faculty, facultyErrors) = Faculty.Create(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.Dean,
            entity.EstablishedDate,
            [],
            []
        );

        if (!string.IsNullOrEmpty(facultyErrors))
            throw new AcademyException(ExceptionType.InvalidRequest, facultyErrors);

        return faculty!;
    }

    public async Task<Faculty?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Faculties
            .AsNoTracking()
            .Include(f => f.Groups)
            .Include(f => f.Departments)
            .FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new AcademyException(ExceptionType.NotFound, "FacultyNotFound");

        return CreateFaculty(entity);
    }

    public async Task<ICollection<Faculty>> GetAllAsync()
    {
        var entities = await _context.Faculties
            .AsNoTracking()
            .Include(f => f.Groups)
            .Include(f => f.Departments)
            .ToListAsync();

        if (!entities.Any())
            throw new AcademyException(ExceptionType.NotFound, "NoFacultiesFound");

       return entities
           .Select(CreateFaculty)
           .ToList();
    }

    public async Task AddAsync(Faculty faculty)
    {
        var entity = new FacultyEntity
        {
            Name = faculty.Name,
            Description = faculty.Description,
            Dean = faculty.Dean,
            EstablishedDate = faculty.EstablishedDate
        };

        await _context.Faculties
            .AddAsync(entity)
            .ConfigureAwait(false);;
    }

    public async Task UpdateAsync(Faculty faculty)
    {
        var entity = await _context.Faculties.FindAsync(faculty.Id)
            ?? throw new AcademyException(ExceptionType.NotFound, "FacultyNotFound");
        
        entity.Name = faculty.Name;
        entity.Description = faculty.Description;
        entity.Dean = faculty.Dean;
        entity.EstablishedDate = faculty.EstablishedDate;
        
        _context.Faculties.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Faculties.FindAsync(id)
            ?? throw new AcademyException(ExceptionType.NotFound, "FacultyNotFound");
        
        _context.Faculties.Remove(entity);
    }

    public async Task<ICollection<Faculty>> FindAsync(Expression<Func<FacultyEntity, bool>> predicate)
    {
        var entities = await _context.Faculties
            .AsNoTracking()
            .Include(f => f.Groups)
            .Include(f => f.Departments)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any()) return new List<Faculty>();

        return entities
            .Select(CreateFaculty)
            .ToList();
    }
}
