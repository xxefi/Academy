namespace Academy.Domain.Entities;

public class FacultyEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Dean { get; set; } = string.Empty;
    public DateTime EstablishedDate { get; set; }
    public ICollection<DepartmentEntity> Departments { get; set; } = [];
    public ICollection<GroupEntity> Groups { get; set; } = [];
}