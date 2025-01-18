using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly AcademyContext _context;

    public GroupRepository(AcademyContext context)
        =>  _context = context;

    private Group CreateGroup(GroupEntity entity)
    {
        var (group, groupErrors) = Group.Create(
            entity.Id,
            entity.Name,
            null,
            null!,
            null,
            null!,
            [],
            []
        );

        if (!string.IsNullOrEmpty(groupErrors))
            throw new AcademyException(ExceptionType.InvalidRequest, groupErrors);

        return group!;
    }

    public async Task<Group?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Groups
            .AsNoTracking()
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new AcademyException(ExceptionType.NotFound, "GroupNotFound");

        return CreateGroup(entity);
    }

    public async Task<ICollection<Group>> GetAllAsync()
    {
        var entities = await _context.Groups
            .AsNoTracking()
            .Include(g => g.Students)
            .ToListAsync();

        if (!entities.Any())
            throw new AcademyException(ExceptionType.NotFound, "GroupNoGroups");

       return entities
           .Select(CreateGroup)
           .ToList();
    }

    public async Task AddAsync(Group group)
    {
        var entity = new GroupEntity
        {
            Name = group.Name,
            FacultyId = group.FacultyId ?? Guid.Empty,
            TeacherId = group.TeacherId ?? Guid.Empty,
        };

        await _context.Groups
            .AddAsync(entity)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(Group group)
    {
        var entity = await _context.Groups.FindAsync(group.Id)
            ?? throw new AcademyException(ExceptionType.NotFound, "GroupNotFound");
        
        entity.Name = group.Name;
        entity.FacultyId = group.FacultyId ?? Guid.Empty;
        entity.TeacherId = group.TeacherId;
        
        _context.Groups.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Groups.FindAsync(id)
            ?? throw new AcademyException(ExceptionType.NotFound, "GroupNotFound");
        
        _context.Groups.Remove(entity);
    }

    public async Task<ICollection<Group>> FindAsync(Expression<Func<GroupEntity, bool>> predicate)
    {
        var entities = await _context.Groups
            .AsNoTracking()
            .Include(g => g.Students)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any())
            return new List<Group>();
        
        return entities
            .Select(CreateGroup)
            .ToList();
    }
}
