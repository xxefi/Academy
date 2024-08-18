using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;

namespace Academy.Server.Services.Interfaces;

public interface IStudentService
{
    public Task<IEnumerable<Student>> GetAllStudentsAsync();
    public Task<Student> AddStudentAsync(StudentDto student);
    public Task<Student> DeleteStudentAsync(Guid studentId);
    public Task<Student> UpdateStudentAsync(Guid studentId, StudentDto student);
}
