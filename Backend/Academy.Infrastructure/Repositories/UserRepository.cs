using System.Linq.Expressions;
using Academy.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using Academy.Infrastructure.Context;
using static BCrypt.Net.BCrypt;

namespace Academy.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AcademyContext _context;

    public UserRepository(AcademyContext context)
    {
        _context = context;
    }

    private User? CreateUser(UserEntity entity)
    {
        var (user, errors) = User.Create(
            entity.Id,
            entity.Username,
            entity.Password,
            entity.Email,
            null!,
            entity.RoleId,
            entity.RefreshToken,
            entity.RefreshTokenExpiryTime
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);

        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new AcademyException(ExceptionType.NotFound, "UserNotFound");

        return CreateUser(entity);
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        var entities = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .ToListAsync();

        if (!entities.Any())
            throw new AcademyException(ExceptionType.NotFound, "UserNoUsers");

        var users = entities
            .Select(e => CreateUser(e))
            .Where(u => u != null)
            .Cast<User>()
            .ToList();

        return users;
    }

    public async Task AddAsync(User user)
    {
        var entity = new UserEntity
        {
            Username = user.Username,
            Password = HashPassword(user.Password),
            Email = user.Email,
            RoleId = user.Role.Id,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
        };

        await _context.Users.AddAsync(entity);
    }

    public async Task UpdateAsync(User user)
    {
        var entity = await _context.Users.FindAsync(user.Id)
            ?? throw new AcademyException(ExceptionType.NotFound, "UserNotFound");
        
        entity.Username = user.Username;
        entity.Password = user.Password;
        entity.Email = user.Email;
        entity.RoleId = user.RoleId;
        entity.RefreshToken = user.RefreshToken;
        entity.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
        
        _context.Users.Update(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var entity = await _context.Users.FindAsync(id)
            ?? throw new AcademyException(ExceptionType.NotFound, "UserNotFound");
        
        _context.Users.Remove(entity);
    }

    public async Task<ICollection<User>> FindAsync(Expression<Func<UserEntity, bool>> predicate)
    {
        var entities = await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(predicate)
            .ToListAsync();

        if (!entities.Any())
            return new List<User>();

        var users = entities
            .Select(e => CreateUser(e))
            .Where(u => u != null)
            .Cast<User>()
            .ToList();

        return users;
    }
}
