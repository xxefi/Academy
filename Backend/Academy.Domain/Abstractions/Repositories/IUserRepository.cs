using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<ICollection<User>> GetAllAsync();
    Task AddAsync(User user);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(User user);
    Task<ICollection<User>> FindAsync(Expression<Func<UserEntity, bool>> predicate);
}