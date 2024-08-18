using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Services.Classes;

public class FacultyService : IFacultyService
{
    private readonly AcademyContext _context;

    public FacultyService(AcademyContext context) => _context = context;

    public async Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
    {
        return await _context.Faculties.ToListAsync();
    }

    public async Task<Faculty> AddFacultyAsync(FacultyDto faculty)
    {
        var newFaculty = new Faculty { Name = faculty.Name };
        try
        {
            await _context.Faculties.AddAsync(newFaculty);
            await _context.SaveChangesAsync();

            return newFaculty;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Faculty> AddGroupToFacultyAsync(Guid facultyId, Guid groupId)
    {
        var faculty = await _context.Faculties
            .Include(g => g.Groups)
            .FirstOrDefaultAsync(f => f.Id == facultyId) 
            ?? throw new Exception($"Faculty with Id '{facultyId}' not found");

        var group = await _context.Groups.FindAsync(groupId)
            ?? throw new Exception($"Group with Id '{groupId}' not found");
        try
        {
            faculty.Groups.Add(group);
            _context.Faculties.Update(faculty);
            await _context.SaveChangesAsync();  

            return faculty;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Faculty> DeleteFacultyAsync(Guid facultyId)
    {
        var faculty = await _context.Faculties.FindAsync(facultyId)
            ?? throw new Exception($"Faculty with Id '{facultyId}' not found");
        try
        {
            _context.Faculties.Remove(faculty);
            await _context.SaveChangesAsync();

            return faculty;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Faculty> DeleteGroupFromFacultyAsync(Guid facultyId, Guid groupId)
    {
        var faculty = await _context.Faculties
                .Include(f => f.Groups)
                .FirstOrDefaultAsync(f => f.Id == facultyId)
                ?? throw new Exception($"Faculty with Id '{facultyId}' not found");

        var group = faculty.Groups.FirstOrDefault(g => g.Id == groupId)
            ?? throw new Exception($"Group with Id '{groupId}' not found");
        try
        {

            group.FacultyId = null;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return faculty;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Faculty> UpdateFacultyAsync(Guid facultyId, FacultyDto faculty)
    {
        try
        {
            var faculties = await _context.Faculties.FindAsync(facultyId)
                ?? throw new Exception($"Faculty with Id '{facultyId}' not found");
            faculties.Name = faculty.Name;
            _context.Faculties.Update(faculties);
            await _context.SaveChangesAsync();

            return faculties;
        }
        catch
        {
            throw;
        }
    }
}
