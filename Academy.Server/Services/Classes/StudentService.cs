using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Academy.Server.Services.Classes;

public class StudentService : IStudentService
{
    private readonly AcademyContext _context;

    public StudentService(AcademyContext context) => _context = context;

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students.ToListAsync();
    }

    public async Task<Student> AddStudentAsync(StudentDto student)
    {
        var group = await _context.Groups.FindAsync(student.GroupId)
            ?? throw new Exception($"Group with Id '{student.GroupId}' not found");
        var newStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = student.FirstName,
            LastName = student.LastName,
            GroupId = student.GroupId,
        };
        try
        {
            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();

            return newStudent;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Student> DeleteStudentAsync(Guid studentId)
    {
        var students = await _context.Students.FindAsync(studentId)
            ?? throw new Exception($"Student with Id '{studentId}' not found");
        try
        {
            _context.Students.Remove(students);
            await _context.SaveChangesAsync();
            return students;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Student> UpdateStudentAsync(Guid studentId, StudentDto student)
    {
        var students = await _context.Students.FindAsync(studentId)
            ?? throw new Exception($"Student with Id '{studentId}' not found");
        try
        {
            students.FirstName = student.FirstName;
            students.LastName = student.LastName;

            _context.Students.Update(students);
            await _context.SaveChangesAsync();
            return students;
        }
        catch
        {
            throw;
        }
    }
}
