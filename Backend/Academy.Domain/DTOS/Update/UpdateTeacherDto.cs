namespace Academy.Domain.DTOS.Update;

public class UpdateTeacherDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public ICollection<Guid> GroupIds { get; set; } = [];
}