using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id);
    Task<ICollection<Group>> GetAllAsync();
    Task AddAsync(Group group);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Group group);
    Task<ICollection<Group>> FindAsync(Expression<Func<GroupEntity, bool>> predicate);
}