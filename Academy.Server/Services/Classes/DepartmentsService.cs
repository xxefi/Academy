using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using ApiFirst.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Services.Classes;

public class DepartmentsService : IDepartmentsService
{
    private readonly AcademyContext _context;

    public DepartmentsService(AcademyContext context) => _context = context;
    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department> AddDepartmentAsync(DepartmentDto department)
    {
        var departments = new Department { Name = department.Name };
        try
        {
            await _context.Departments.AddAsync(departments);
            await _context.SaveChangesAsync();

            return departments;
        }
        catch
        {
            throw;
        }

    }
    public async Task<Department> AddTeacherToDepartmentAsync(Guid departmentId, Guid teacherId)
    {
        var department = await _context.Departments
            .Include(t => t.Teachers)
            .FirstOrDefaultAsync(d => d.Id == departmentId)
            ?? throw new Exception($"Department with Id '{departmentId}' not found");

        var teacher = await _context.Teachers.FindAsync(teacherId)
            ?? throw new Exception($"Teacher with Id '{teacherId} not found'");
        try
        {
            department.Teachers.Add(teacher);
            _context.Departments.Update(department);

            await _context.SaveChangesAsync();
            return department;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Department> DeleteDepartmentAsync(Guid departmentId)
    {
        var department = await _context.Departments.FindAsync(departmentId)
            ?? throw new Exception($"Department with Id '{departmentId}' not found");
        try
        {
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return department;
        }
        catch
        {
            throw;
        }
       
    }
    public async Task<Department> DeleteTeacherFromDepartmentAsync(Guid departmentId, Guid teacherId)
    {
        var department = await _context.Departments
            .Include(t => t.Teachers)
            .FirstOrDefaultAsync(d => d.Id == departmentId) ??
            throw new Exception($"Department with Id '{departmentId}' not found");

        var teacher = department.Teachers.FirstOrDefault(t => t.Id == teacherId)
            ?? throw new Exception($"Teacher with Id '{teacherId}' not found");
        try
        {
            department.Teachers.Remove(teacher);
            _context.Departments.Update(department);

            await _context.SaveChangesAsync();
            return department;
        }
        catch
        {
            throw;
        }
    }
    public async Task<Department> UpdateDepartmentAsync(Guid departmentId, DepartmentDto department)
    {
        var departments = await _context.Departments.FindAsync(departmentId)
            ?? throw new Exception($"Department with Id '{departmentId}' not found");
        try
        {
            departments.Name = department.Name;
            _context.Departments.Update(departments);

            await _context.SaveChangesAsync();

            return departments;
        }
        catch
        {
            throw;
        }
    }
}
