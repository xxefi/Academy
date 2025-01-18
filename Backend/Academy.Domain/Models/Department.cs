namespace Academy.Domain.Models;

public class Department
{
    public const int MAX_NAME_LENGTH = 100;
    public const int MAX_DESCRIPTION_LENGTH = 500;
    public const int MAX_HEAD_NAME_LENGTH = 100;

    public const string DEPARTMENT_NAME_EMPTY_KEY = "DepartmentNameEmpty";
    public const string DEPARTMENT_DESCRIPTION_EMPTY_KEY = "DepartmentDescriptionEmpty";
    public const string DEPARTMENT_HEAD_EMPTY_KEY = "DepartmentHeadEmpty";
    public const string DEPARTMENT_NAME_LENGTH_KEY = "DepartmentNameLength";
    public const string DEPARTMENT_DESCRIPTION_LENGTH_KEY = "DepartmentDescriptionLength";
    public const string DEPARTMENT_HEAD_LENGTH_KEY = "DepartmentHeadLength";
    public const string DEPARTMENT_NAME_DESCRIPTION_HEAD_LENGTH_KEY = "DepartmentNameDescriptionHeadLength";
    public const string DEPARTMENT_TEACHERS_EMPTY_KEY = "DepartmentTeachersEmpty";

    public Department(Guid id, string name, string description, string head, DateTime establishedDate, Guid? facultyId, ICollection<Faculty> faculties, ICollection<Teacher> teachers)
    {
        Id = id;
        Name = name;
        Description = description;
        Head = head;
        EstablishedDate = establishedDate;
        FacultyId = facultyId;
        Faculties = faculties;
        Teachers = teachers ?? [];
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Head { get; }
    public DateTime EstablishedDate { get; }
    public Guid? FacultyId { get; }
    public ICollection<Faculty> Faculties { get; }
    public ICollection<Teacher> Teachers { get; }

    public static (Department? Department, string Errors) Create(
        Guid id, 
        string name, 
        string description, 
        string head, 
        DateTime establishedDate, 
        Guid? facultyId, 
        ICollection<Faculty> faculties, 
        ICollection<Teacher> teachers)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(name) ? DEPARTMENT_NAME_EMPTY_KEY : string.Empty,
            name.Length > MAX_NAME_LENGTH ? DEPARTMENT_NAME_LENGTH_KEY : string.Empty,
            string.IsNullOrWhiteSpace(description) ? DEPARTMENT_DESCRIPTION_EMPTY_KEY : string.Empty,
            description.Length > MAX_DESCRIPTION_LENGTH ? DEPARTMENT_DESCRIPTION_LENGTH_KEY : string.Empty,
            string.IsNullOrWhiteSpace(head) ? DEPARTMENT_HEAD_EMPTY_KEY : string.Empty,
            head.Length > MAX_HEAD_NAME_LENGTH ? DEPARTMENT_HEAD_LENGTH_KEY : string.Empty,
        }.FirstOrDefault(e => !string.IsNullOrEmpty(e));

        return !string.IsNullOrEmpty(errors)
            ? (null, string.Join("\n", errors))
            : (new Department(id, name, description, head, establishedDate, facultyId, faculties, (teachers ?? []).ToList().AsReadOnly()), string.Empty);
    }
}