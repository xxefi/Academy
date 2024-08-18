using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Data.Models.Dtos;
using Academy.Server.Services.Interfaces;
using ApiFirst.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Academy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentsController(IDepartmentsService departmentsService) => _departmentsService = departmentsService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllDepartments()
    {
        var departments = await _departmentsService.GetAllDepartmentsAsync();
        return Ok(departments);
    }

    [HttpPost("Add")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDto departmentDto)
    {
        if (departmentDto == null)
            return BadRequest("Invalid data");
        try
        {
            var department = await _departmentsService.AddDepartmentAsync(departmentDto);
            return Ok(department);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    [HttpPut("Update")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> UpdateDepartment(Guid departmentId, [FromBody] DepartmentDto departmentDto)
    {
        if (departmentDto == null)
            return BadRequest("Invalid data");
        try
        {
            var department = await _departmentsService.UpdateDepartmentAsync(departmentId, departmentDto);

            return Ok(department);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("Remove")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> DeleteDepartment(Guid departmentId)
    {
        try
        {
            var department = await _departmentsService.DeleteDepartmentAsync(departmentId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("AddTeacher")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> AddTeacherToDepartment(Guid departmentId, Guid teacherId)
    {
        try
        {
            var department = await _departmentsService.AddTeacherToDepartmentAsync(departmentId, teacherId);
            return Ok(department);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }
    [HttpDelete("RemoveTeacher")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> DeleteTeacherFromDepartment(Guid departmentId, Guid teacherId)
    {
        try
        {
            var department = await _departmentsService.DeleteTeacherFromDepartmentAsync(departmentId, teacherId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
       
    }
}