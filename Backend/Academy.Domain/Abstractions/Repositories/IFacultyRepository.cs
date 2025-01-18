using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IFacultyRepository
{
    Task<Faculty?> GetByIdAsync(Guid id);
    Task<ICollection<Faculty>> GetAllAsync();
    Task AddAsync(Faculty faculty);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Faculty faculty);
    Task<ICollection<Faculty>> FindAsync(Expression<Func<FacultyEntity, bool>> predicate);
}