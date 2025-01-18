using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IFacultyService
{
    Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync();
    Task<FacultyDto?> GetFacultyByIdAsync(Guid id);
    Task<FacultyDto?> GetFacultyByNameAsync(string name);
    Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto createFacultyDto);
    Task<FacultyDto> UpdateFacultyAsync(Guid id, UpdateFacultyDto updateFacultyDto);
    Task<bool> DeleteFacultyAsync(Guid id);
    Task<IEnumerable<DepartmentDto>> GetFacultyDepartmentsAsync(Guid facultyId);
    Task<FacultyDto> AddDepartmentToFacultyAsync(Guid facultyId, Guid departmentId);
    Task<bool> RemoveDepartmentFromFacultyAsync(Guid facultyId, Guid departmentId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetDepartmentsCountAsync(Guid facultyId);
    Task<IEnumerable<GroupDto>> GetFacultyGroupsAsync(Guid facultyId);
    Task<int> GetStudentsCountAsync(Guid facultyId);
    Task<IEnumerable<FacultyDto>> GetFacultiesPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalFacultiesCountAsync();
    Task<IEnumerable<DepartmentDto>> GetFacultyDepartmentsPageAsync(Guid facultyId, int pageNumber, int pageSize);
    Task<IEnumerable<GroupDto>> GetFacultyGroupsPageAsync(Guid facultyId, int pageNumber, int pageSize);
    Task<int> GetTotalGroupsCountAsync(Guid facultyId);
}