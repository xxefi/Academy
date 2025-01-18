using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id);
    Task<DepartmentDto?> GetDepartmentByNameAsync(string name);
    Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto);
    Task<DepartmentDto> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDepartmentDto);
    Task<bool> DeleteDepartmentAsync(Guid id);
    Task<IEnumerable<TeacherDto>> GetDepartmentTeachersAsync(Guid departmentId);
    Task<DepartmentDto> AddTeacherToDepartmentAsync(Guid departmentId, Guid teacherId);
    Task<bool> RemoveTeacherFromDepartmentAsync(Guid departmentId, Guid teacherId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetTeachersCountAsync(Guid departmentId);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsByFacultyAsync(Guid facultyId);
    Task<DepartmentDto> TransferTeacherAsync(Guid teacherId, Guid sourceDepartmentId, Guid targetDepartmentId);
    Task<IEnumerable<TeacherDto>> GetTeachersPageAsync(Guid departmentId, int pageNumber, int pageSize);
    Task<IEnumerable<DepartmentDto>> GetDepartmentsPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalDepartmentsCountAsync();
    Task<int> GetTotalTeachersCountAsync(Guid departmentId);
    Task<IEnumerable<TeacherDto>> GetDepartmentTeachersPageAsync(Guid departmentId, int pageNumber, int pageSize);
}