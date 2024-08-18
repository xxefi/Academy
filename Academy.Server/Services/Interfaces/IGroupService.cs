using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;

namespace Academy.Server.Services.Interfaces;

public interface IGroupService
{
    public Task<IEnumerable<Group>> GetAllGroupsAsync();
    public Task<Group> AddGroupAsync(GroupDto group);
    public Task<Group> DeleteGroupAsync(Guid groupId);
    public Task<Group> UpdateGroupAsync(Guid id, GroupDto group);
    public Task<Group> ChangeTeacherFromGroupAsync(Guid groupId, Guid currentTeacherId, Guid newTeacherId);
    public Task<Group> AddStudentToGroupAsync(Guid groupId, StudentDto student);
    public Task<Group> RemoveStudentFromGroupAsync(Guid groupid, Guid studentId);
}
