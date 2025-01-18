using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;

namespace Academy.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AcademyContext _context;

    public RoleRepository(AcademyContext context)
        =>  _context = context;

    private Role MapToDomainModel(RoleEntity entity)
    {
        var users = entity.Users
            .Select(userEntity => new User(
                userEntity.Id,
                userEntity.Username,
                userEntity.Password,
                userEntity.Email,
                null,
                userEntity.RoleId,
                userEntity.RefreshToken,
                userEntity.RefreshTokenExpiryTime
            ))
            .ToList();

        var (role, errors) = Role.Create(
            entity.Id,
            entity.Name,
            entity.Description,
            users
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);

        return role;
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Roles
            .AsNoTracking()
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id)
            .ConfigureAwait(false)
            ?? throw new AcademyException(ExceptionType.NotFound, "NoRoleFound");

        return MapToDomainModel(entity);
    }

    public async Task<ICollection<Role>> GetAllAsync()
    {
        var entities = await _context.Roles
            .AsNoTracking()
            .Include(r => r.Users)
            .ToListAsync()
            .ConfigureAwait(false);

        return entities.Any()
            ? entities.Select(MapToDomainModel).ToList()
            : throw new AcademyException(ExceptionType.NotFound, "RoleNoRoles");
    }

    public async Task AddAsync(Role role)
    {
        var entity = new RoleEntity
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };

        await _context.Roles
            .AddAsync(entity)
            .ConfigureAwait(false);
    }

    public async Task UpdateAsync(Role role)
    {
        var entity = await _context.Roles
            .FindAsync(role.Id)
            .ConfigureAwait(false)
            ?? throw new AcademyException(ExceptionType.NotFound, "RoleNotFound");
        
        entity.Name = role.Name;
        entity.Description = role.Description;
        
        _context.Roles.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Roles
                .FindAsync(id)
                .ConfigureAwait(false)
                ?? throw new AcademyException(ExceptionType.NotFound, "RoleNotFound");
        
        _context.Roles.Remove(entity);
    }

    public async Task<ICollection<Role>> FindAsync(Expression<Func<RoleEntity, bool>> predicate)
    {
        var entities = await _context.Roles
            .AsNoTracking()
            .Include(r => r.Users)
            .Where(predicate)
            .ToListAsync()
            .ConfigureAwait(false);

        return entities.Select(MapToDomainModel).ToList();
    }
}
