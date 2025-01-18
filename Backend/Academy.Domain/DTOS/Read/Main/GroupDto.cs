namespace Academy.Domain.DTOS.Read.Main;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? FacultyId { get; set; }
    public Guid? TeacherId { get; set; }
    public ICollection<Guid> StudentIds { get; set; } = [];
}