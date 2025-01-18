using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface ITeacherService
{
    Task<IEnumerable<TeacherDto>> GetAllTeachersAsync();
    Task<TeacherDto?> GetTeacherByIdAsync(Guid id);
    Task<TeacherDto?> GetTeacherByNameAsync(string firstName, string lastName);
    Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto);
    Task<TeacherDto> UpdateTeacherAsync(Guid id, UpdateTeacherDto updateTeacherDto);
    Task<bool> DeleteTeacherAsync(Guid id);
    Task<IEnumerable<GroupDto>> GetTeacherGroupsAsync(Guid teacherId);
    Task<DepartmentDto?> GetTeacherDepartmentAsync(Guid teacherId);
    Task<TeacherDto> AssignGroupToTeacherAsync(Guid teacherId, Guid groupId);
    Task<TeacherDto> RemoveGroupFromTeacherAsync(Guid teacherId, Guid groupId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<IEnumerable<TeacherDto>> GetTeachersPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalTeachersCountAsync();
}