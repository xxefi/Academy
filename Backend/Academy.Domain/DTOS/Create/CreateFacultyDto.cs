namespace Academy.Domain.DTOS.Create;

public class CreateFacultyDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Dean { get; set; } = string.Empty;
    public Guid GroupId { get; set; }  
}  