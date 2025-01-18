namespace Academy.Domain.Entities;

public class DepartmentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Head { get; set; } = string.Empty;
    public DateTime EstablishedDate { get; set; }
    public Guid? FacultyId { get; set; } 
    public FacultyEntity Faculty { get; set; } = null!;
    public ICollection<TeacherEntity> Teachers { get; set; } = [];
}