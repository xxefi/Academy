namespace Academy.Domain.DTOS.Read.Main;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public ICollection<Guid> GroupIds { get; set; } = [];
}