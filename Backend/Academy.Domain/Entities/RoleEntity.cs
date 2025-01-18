namespace Academy.Domain.Entities;

public class RoleEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<UserEntity> Users { get; set; } = [];
}