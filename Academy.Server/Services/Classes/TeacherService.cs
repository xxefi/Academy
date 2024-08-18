using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Services.Classes;

public class TeacherService : ITeacherService
{
    private readonly AcademyContext _context;

    public TeacherService(AcademyContext context) => _context = context;

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        return await _context.Teachers.ToListAsync();
    }

    public async Task<Teacher> AddTeacherAsync(TeacherDto teacher)
    {
        if (await _context.Departments.FindAsync(teacher.DepartmentId) == null)
            throw new Exception($"Department with Id '{teacher.DepartmentId}' not found");
        var newTeacher = new Teacher
        {
            Id = Guid.NewGuid(),
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            DepartmentId = teacher.DepartmentId,
        };
        try
        {
            await _context.Teachers.AddAsync(newTeacher);
            await _context.SaveChangesAsync();

            return newTeacher;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Teacher> DeleteTeacherAsync(Guid teacherId)
    {
        var teachers = await _context.Teachers.FindAsync(teacherId)
            ?? throw new Exception($"Teacher with Id '{teacherId}' not found");
        try
        {
            _context.Teachers.Remove(teachers);
            await _context.SaveChangesAsync();
            return teachers;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Teacher> UpdateTeacherAsync(Guid teacherId, TeacherDto teacher)
    {
        var teachers = await _context.Teachers.FindAsync(teacherId)
            ?? throw new Exception($"Teacher with Id '{teacherId}' not found");

        try
        {
            teachers.FirstName = teacher.FirstName;
            teachers.LastName = teacher.LastName;

            _context.Teachers.Update(teachers);
            await _context.SaveChangesAsync();
            return teachers;
        }
        catch
        {
            throw;
        }
    }
}
