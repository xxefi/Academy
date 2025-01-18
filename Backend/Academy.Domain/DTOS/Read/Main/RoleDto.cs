namespace Academy.Domain.DTOS.Read.Main;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Guid> UserIds { get; set; }
}