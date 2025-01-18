namespace Academy.Domain.DTOS.Update;

public class UpdateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
}