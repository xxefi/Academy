namespace Academy.Domain.DTOS.Read.Main;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
}