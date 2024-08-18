using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;

namespace Academy.Server.Services.Interfaces;

public interface ITeacherService
{
    public Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    public Task<Teacher> AddTeacherAsync(TeacherDto teacher);
    public Task<Teacher> DeleteTeacherAsync(Guid teacherId);
    public Task<Teacher> UpdateTeacherAsync(Guid teacherId, TeacherDto teacher);
}
