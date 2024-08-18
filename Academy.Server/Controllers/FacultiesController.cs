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
public class FacultiesController : ControllerBase
{
    private readonly IFacultyService _facultyService;
    public FacultiesController(IFacultyService facultyService) => _facultyService = facultyService;

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllFaculties()
    {
        try
        {
            var faculties = await _facultyService.GetAllFacultiesAsync();
            return Ok(faculties);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Add")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> CreateFaculty([FromBody] FacultyDto facultyDto)
    {
        if (facultyDto == null)
            return BadRequest("Invalid data");
        try
        {
            var faculty = await _facultyService.AddFacultyAsync(facultyDto);
            return Ok(faculty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("Remove")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveFaculty(Guid facultyId)
    {
        try
        {
            var faculty = await _facultyService.DeleteFacultyAsync(facultyId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("Update")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> UpdateFaculty(Guid facultyId, [FromBody] FacultyDto facultyDto)
    {
        if (facultyDto == null)
            return BadRequest("Invalid data");
        try
        {
            var faculty = await _facultyService.UpdateFacultyAsync(facultyId, facultyDto);

            return Ok(faculty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("AddGroup")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> AddGroupToFaculty(Guid facultyId, Guid groupId)
    {
        try
        {
            var faculty = await _facultyService.AddGroupToFacultyAsync(facultyId, groupId);
            return Ok(faculty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("RemoveGroup")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveGroupFromFaculty(Guid facultyId, Guid groupId)
    {
        try
        {
            var faculty = await _facultyService.DeleteGroupFromFacultyAsync(facultyId, groupId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
