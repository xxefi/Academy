namespace Academy.Domain.Entities;

public class TeacherEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public DepartmentEntity? Department { get; set; }
    public ICollection<GroupEntity> Groups { get; set; } = [];
}