using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;

namespace Academy.Server.Services.Interfaces;

public interface IFacultyService
{
    public Task<IEnumerable<Faculty>> GetAllFacultiesAsync();
    public Task<Faculty> AddFacultyAsync(FacultyDto faculty);
    public Task<Faculty> DeleteFacultyAsync(Guid facultyId);
    public Task<Faculty> UpdateFacultyAsync(Guid facultyId, FacultyDto faculty);
    public Task<Faculty> AddGroupToFacultyAsync(Guid facultyId, Guid groupId);
    public Task<Faculty> DeleteGroupFromFacultyAsync(Guid facultyId, Guid groupId);
}
