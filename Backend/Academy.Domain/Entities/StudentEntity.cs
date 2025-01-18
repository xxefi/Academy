namespace Academy.Domain.Entities;

public class StudentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public GroupEntity Group { get; set; } = null!;
}