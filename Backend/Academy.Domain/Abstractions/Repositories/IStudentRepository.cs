using System.Linq.Expressions;
using Academy.Domain.Entities;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<ICollection<Student>> GetAllAsync();
    Task AddAsync(Student student);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(Student student);
    Task<ICollection<Student>> FindAsync(Expression<Func<StudentEntity, bool>> predicate);
}