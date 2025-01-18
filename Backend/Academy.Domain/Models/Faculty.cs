namespace Academy.Domain.Models;

public class Faculty
{
    public const int MAX_NAME_LENGTH = 100;
    public const int MAX_DESCRIPTION_LENGTH = 500;
    public const int MAX_DEAN_NAME_LENGTH = 100;

    public const string FACULTY_NAME_EMPTY_KEY = "FacultyNameEmpty";
    public const string FACULTY_DESCRIPTION_EMPTY_KEY = "FacultyDescriptionEmpty";
    public const string FACULTY_DEAN_EMPTY_KEY = "FacultyDeanEmpty";
    public const string FACULTY_NAME_LENGTH_KEY = "FacultyNameLength";
    public const string FACULTY_DESCRIPTION_LENGTH_KEY = "FacultyDescriptionLength";
    public const string FACULTY_DEAN_LENGTH_KEY = "FacultyDeanLength";
    public const string FACULTY_NAME_DESCRIPTION_DEAN_LENGTH_KEY = "FacultyNameDescriptionDeanLength";
    public const string FACULTY_GROUPS_EMPTY_KEY = "FacultyGroupsEmpty";

    public Faculty(
        Guid id, 
        string name, 
        string description, 
        string dean, 
        DateTime establishedDate, 
        ICollection<Group> groups,
        ICollection<Department> departments)
    {
        Id = id;
        Name = name;
        Description = description;
        Dean = dean;
        EstablishedDate = establishedDate;
        Groups = groups ?? [];
        Departments = departments ?? [];
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Dean { get; }
    public DateTime EstablishedDate { get; }
    public ICollection<Group> Groups { get; }
    public ICollection<Department> Departments { get; }

    public static (Faculty? Faculty, string Errors) Create(
        Guid id, 
        string name, 
        string description, 
        string dean, 
        DateTime establishedDate, 
        ICollection<Group> groups,
        ICollection<Department> departments)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(name) ? FACULTY_NAME_EMPTY_KEY : string.Empty,
            string.IsNullOrWhiteSpace(description) ? FACULTY_DESCRIPTION_EMPTY_KEY : string.Empty,
            string.IsNullOrWhiteSpace(dean) ? FACULTY_DEAN_EMPTY_KEY : string.Empty,
            name.Length > MAX_NAME_LENGTH && description.Length > MAX_DESCRIPTION_LENGTH && dean.Length > MAX_DEAN_NAME_LENGTH
                ? $"{FACULTY_NAME_DESCRIPTION_DEAN_LENGTH_KEY}: Name({MAX_NAME_LENGTH}), Description({MAX_DESCRIPTION_LENGTH}), Dean({MAX_DEAN_NAME_LENGTH})"
                : string.Empty,
            name.Length > MAX_NAME_LENGTH && (description.Length <= MAX_DESCRIPTION_LENGTH || dean.Length <= MAX_DEAN_NAME_LENGTH)
                ? $"{FACULTY_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}"
                : string.Empty,
            description.Length > MAX_DESCRIPTION_LENGTH && (name.Length <= MAX_NAME_LENGTH || dean.Length <= MAX_DEAN_NAME_LENGTH)
                ? $"{FACULTY_DESCRIPTION_LENGTH_KEY}:{MAX_DESCRIPTION_LENGTH}"
                : string.Empty,
            dean.Length > MAX_DEAN_NAME_LENGTH && (name.Length <= MAX_NAME_LENGTH || description.Length <= MAX_DESCRIPTION_LENGTH)
                ? $"{FACULTY_DEAN_LENGTH_KEY}:{MAX_DEAN_NAME_LENGTH}"
                : string.Empty,
            (groups == null || !groups.Any()) ? FACULTY_GROUPS_EMPTY_KEY : string.Empty
        }.Where(e => !string.IsNullOrEmpty(e)).ToList();

        return errors.Count > 0
            ? (null, string.Join("\n", errors))
            : (new Faculty(
                id, 
                name, 
                description, 
                dean, 
                establishedDate, 
                (groups ?? []).ToList().AsReadOnly(),
                (departments ?? []).ToList().AsReadOnly()), 
              string.Empty);
    }
}