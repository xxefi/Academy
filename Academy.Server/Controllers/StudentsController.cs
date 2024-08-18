using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Academy.Server.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Academy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService) => _studentService = studentService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllStudents()
    {
        try
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Add")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> AddStudent([FromBody] StudentDto studentDto)
    {
        if (studentDto == null)
            return BadRequest("Invalid data");
        try
        {
            var students = await _studentService.AddStudentAsync(studentDto);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("Remove")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveStudent(Guid studentId)
    {
        try
        {
            var students = await _studentService.DeleteStudentAsync(studentId);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("Update")]
    [Authorize(Roles = "appadmin")]
    public async Task <IActionResult> UpdateStudent(Guid studentId, [FromBody] StudentDto studentDto)
    {
        if (studentDto == null)
            return BadRequest("Invalid data");
        try
        {
            var student = await _studentService.UpdateStudentAsync(studentId, studentDto);
            return Ok(student);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
