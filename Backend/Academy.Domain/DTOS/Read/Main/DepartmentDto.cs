namespace Academy.Domain.DTOS.Read.Main;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public DateTime EstablishedDate { get; set; }
    public ICollection<Guid> TeacherIds { get; set; } = [];
    public Guid FacultyId { get; set; }
}