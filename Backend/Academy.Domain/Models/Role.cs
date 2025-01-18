namespace Academy.Domain.Models;

public class Role
{
    public const int MAX_NAME_LENGTH = 50;
    public const int MAX_DESCRIPTION_LENGTH = 200;

    public const string ROLE_NAME_EMPTY_KEY = "RoleNameEmpty";
    public const string ROLE_NAME_LENGTH_KEY = "RoleNameLength";
    public const string ROLE_DESCRIPTION_KEY = "RoleDescriptionLength";
    public const string ROLE_NAME_AND_DESCRIPTION_KEY = "RoleNameAndDescriptionLength";

    public Role(Guid id, string name, string description, ICollection<User> users)
    {
        Id = id;
        Name = name;
        Description = description;
        Users = users ?? [];
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public ICollection<User> Users { get; }

    public static (Role? Role, string Errors) Create(
        Guid id, 
        string name, 
        string description, 
        ICollection<User> users)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(name) ? ROLE_NAME_EMPTY_KEY : string.Empty,
            name.Length > MAX_NAME_LENGTH  ? ROLE_NAME_LENGTH_KEY : string.Empty,
            description.Length > MAX_DESCRIPTION_LENGTH ? ROLE_DESCRIPTION_KEY : string.Empty
        }.FirstOrDefault(e => !string.IsNullOrEmpty(e));

        return !string.IsNullOrEmpty(errors)
            ? (null, string.Join("\n", errors))
            : (new Role(id, name, description, users.ToList().AsReadOnly()), string.Empty);
    }
}