using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;

namespace Academy.Server.Services.Interfaces;

public interface IDepartmentsService
{
    public Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    public Task<Department> AddDepartmentAsync(DepartmentDto department);
    public Task<Department> UpdateDepartmentAsync(Guid departmentId, DepartmentDto department);
    public Task<Department> DeleteDepartmentAsync(Guid departmentId);
    public Task<Department> AddTeacherToDepartmentAsync(Guid departmentId, Guid teacherId);
    public Task<Department> DeleteTeacherFromDepartmentAsync(Guid departmentId, Guid teacherId);
}
