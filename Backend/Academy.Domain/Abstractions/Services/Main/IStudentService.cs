using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(Guid id);
    Task<StudentDto?> GetStudentByNameAsync(string firstName, string lastName);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto);
    Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto);
    Task<bool> DeleteStudentAsync(Guid id);
    Task<GroupDto?> GetStudentGroupAsync(Guid studentId);
    Task<StudentDto> AddStudentToGroupAsync(Guid studentId, Guid groupId);
    Task<bool> RemoveStudentFromGroupAsync(Guid studentId);
    Task<StudentDto> TransferStudentAsync(Guid studentId, Guid targetGroupId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string firstName, string lastName);
    Task<IEnumerable<StudentDto>> GetStudentsByGroupAsync(Guid groupId);
    Task<IEnumerable<StudentDto>> GetStudentsPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalStudentsCountAsync();
}