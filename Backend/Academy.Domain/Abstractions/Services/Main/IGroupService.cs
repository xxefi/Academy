using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IGroupService
{
    Task<IEnumerable<GroupDto>> GetAllGroupsAsync();
    Task<GroupDto?> GetGroupByIdAsync(Guid id);
    Task<GroupDto?> GetGroupByNameAsync(string name);
    Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto);
    Task<GroupDto> UpdateGroupAsync(Guid id, UpdateGroupDto updateGroupDto);
    Task<bool> DeleteGroupAsync(Guid id);
    Task<IEnumerable<StudentDto>> GetGroupStudentsAsync(Guid groupId);
    Task<GroupDto> AddStudentToGroupAsync(Guid groupId, Guid studentId);
    Task<GroupDto> RemoveStudentFromGroupAsync(Guid groupId, Guid studentId);
    Task<GroupDto> TransferStudentAsync(Guid studentId, Guid sourceGroupId, Guid targetGroupId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetStudentsCountAsync(Guid groupId);
    Task<IEnumerable<GroupDto>> GetGroupsByFacultyAsync(Guid facultyId);
    Task<IEnumerable<GroupDto>> GetGroupsByTeacherAsync(Guid teacherId);
    Task<IEnumerable<GroupDto>> GetGroupsPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalGroupsCountAsync();
    Task<IEnumerable<TeacherDto>> GetGroupTeachersAsync(Guid groupId);
    Task<IEnumerable<StudentDto>> GetGroupStudentsPageAsync(Guid groupId, int pageNumber, int pageSize);
    Task<IEnumerable<TeacherDto>> GetGroupTeachersPageAsync(Guid groupId, int pageNumber, int pageSize);
    Task<int> GetTotalStudentsCountAsync(Guid groupId);
    Task<int> GetTotalTeachersCountAsync(Guid groupId);
}