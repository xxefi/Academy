using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id);
    Task<ICollection<Department>> GetAllAsync();
    Task AddAsync(Department department);
    Task UpdateAsync(IEnumerable<Department> departments);
    Task RemoveAsync(Guid id);
    Task<ICollection<Department>> FindAsync(Expression<Func<DepartmentEntity, bool>> predicate);
}