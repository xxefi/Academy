using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface ITeacherRepository
{
    Task<Teacher?> GetByIdAsync(Guid id);
    Task<ICollection<Teacher>> GetAllAsync();
    Task AddAsync(Teacher teacher);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Teacher teacher);
    Task<ICollection<Teacher>> FindAsync(Expression<Func<TeacherEntity, bool>> predicate);
}