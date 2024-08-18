using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Services.Classes;

public class GroupService : IGroupService
{
    private readonly AcademyContext _context;

    public GroupService(AcademyContext context) => _context = context;

    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await _context.Groups.ToListAsync();
    }
    public async Task<Group> AddGroupAsync(GroupDto group)
    {
        if (await _context.Faculties.FindAsync(group.FacultyId) == null) 
            throw new Exception($"Faculty with Id '{group.FacultyId}' not found");

        var newGroup = new Group
        {
            Id = Guid.NewGuid(),
            Name = group.Name,
            FacultyId = group.FacultyId,
            TeacherId = group.TeacherId
        };
        try
        {
            await _context.Groups.AddAsync(newGroup);
            await _context.SaveChangesAsync();

            return newGroup;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Group> AddStudentToGroupAsync(Guid groupId, StudentDto student)
    {
        var group = await _context.Groups
            .Include(s => s.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId)
            ?? throw new Exception($"Group with Id '{groupId}' not found");

        var newStudent = new Student
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
        };
        try
        {
            group.Students.Add(newStudent);
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return group;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Group> RemoveStudentFromGroupAsync(Guid groupId, Guid studentId)
    {
        var group = await _context.Groups
                .Include(s => s.Students)
                .FirstOrDefaultAsync(g => g.Id == groupId)
                ?? throw new Exception($"Group with Id {groupId} not found");

        var student = group.Students.FirstOrDefault(s => s.Id == studentId)
            ?? throw new Exception($"Student with Id '{studentId}' not found");
        try
        {
            group.Students.Remove(student);
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return group;
        }
        catch
        {
            throw;         
        }
    }
    public async Task<Group> ChangeTeacherFromGroupAsync(Guid currentTeacherId, Guid newTeacherId, Guid groupId)
    {
        var group = await _context.Groups
             .Include(s => s.Students)
             .FirstOrDefaultAsync(g => g.Id == groupId) 
             ?? throw new Exception($"Group with Id '{groupId}' not found");

        if (group.TeacherId != currentTeacherId)
            throw new Exception($"The current teacher with Id '{currentTeacherId}' does not match the teacher assigned to the group.");

        if (await _context.Teachers.FindAsync(newTeacherId) == null)
            throw new Exception($"Teacher with Id '{newTeacherId}' not found");

        try
        {
            group.TeacherId = newTeacherId;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
            return group;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Group> DeleteGroupAsync(Guid groupId)
    {
        var group = await _context.Groups.FindAsync(groupId)
            ?? throw new Exception($"Group with Id '{groupId} not found'");
        try
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return group;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Group> UpdateGroupAsync(Guid groupId, GroupDto group)
    {
        var groups = await _context.Groups.FindAsync(groupId)
            ?? throw new Exception($"Group with Id '{groupId}' not found");
        try
        {
            groups.Name = group.Name;
            _context.Groups.Update(groups);
            await _context.SaveChangesAsync();
            return groups;
        }
        catch
        {
            throw;
        }
    }
}
