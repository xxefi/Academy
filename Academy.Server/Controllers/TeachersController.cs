using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService) => _teacherService = teacherService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTeachers()
    {
        try
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            return Ok(teachers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Add")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> AddTeacher([FromBody] TeacherDto teacherDto)
    {
        if (teacherDto == null)
            return BadRequest("Invalid Data");
        try
        {
            var teachers = await _teacherService.AddTeacherAsync(teacherDto);
            return Ok(teachers);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("Remove")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveTeacher(Guid teacherId)
    {
        try
        {
            var teachers = await _teacherService.DeleteTeacherAsync(teacherId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("Update")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> UpdateTeacher(Guid teacherId, [FromBody] TeacherDto teacherDto)
    {
        if (teacherDto == null)
            return BadRequest("Invalid data");
        try
        {
            var teacher = await _teacherService.UpdateTeacherAsync(teacherId, teacherDto);
            return Ok(teacher);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
