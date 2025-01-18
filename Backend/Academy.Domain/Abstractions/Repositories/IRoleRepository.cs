using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<ICollection<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Role role);
    Task<ICollection<Role>> FindAsync(Expression<Func<RoleEntity, bool>> predicate);
}