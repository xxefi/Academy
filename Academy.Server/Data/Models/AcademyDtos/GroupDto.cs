namespace Academy.Server.Data.Models.AcademyDtos;

public class GroupDto
{
    public string Name { get; set; }
    public Guid FacultyId { get; set; }
    public Guid TeacherId { get; set; }
}
