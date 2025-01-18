namespace Academy.Domain.Entities;

public class GroupEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid FacultyId { get; set; }
    public FacultyEntity Faculty { get; set; } = null!;
    public Guid? TeacherId { get; set; }
    public TeacherEntity? Teacher { get; set; }
    public ICollection<StudentEntity> Students { get; set; } = [];
    public ICollection<TeacherEntity> Teachers { get; set; } = [];
}