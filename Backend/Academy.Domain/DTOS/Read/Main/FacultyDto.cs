namespace Academy.Domain.DTOS.Read.Main;

public class FacultyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Dean { get; set; } = string.Empty;
    public DateTime EstablishedDate { get; set; }
    public ICollection<Guid> DepartmentIds { get; set; } = [];
}