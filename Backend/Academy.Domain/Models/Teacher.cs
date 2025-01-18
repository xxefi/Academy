namespace Academy.Domain.Models;

public class Teacher
{
    public const int MAX_NAME_LENGTH = 50;

    public const string TEACHER_FIRST_NAME_EMPTY_KEY = "TeacherFirstNameEmpty";
    public const string TEACHER_LAST_NAME_EMPTY_KEY = "TeacherLastNameEmpty";
    public const string TEACHER_FIRST_NAME_LENGTH_KEY = "TeacherFirstNameLength";
    public const string TEACHER_LAST_NAME_LENGTH_KEY = "TeacherLastNameLength";
    public const string TEACHER_DEPARTMENT_ID_EMPTY_KEY = "TeacherDepartmentIdEmpty";
    public const string TEACHER_GROUPS_EMPTY_KEY = "TeacherGroupsEmpty";

    public Teacher(
        Guid id, 
        string firstName, 
        string lastName, 
        Guid departmentId, 
        Department department, 
        ICollection<Group> groups)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DepartmentId = departmentId;
        Department = department;
        Groups = groups ?? [];
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Guid DepartmentId { get; }
    public Department Department { get; }
    public ICollection<Group> Groups { get; }

    public static (Teacher? Teacher, string Errors) Create(
        Guid id, 
        string firstName, 
        string lastName, 
        Guid departmentId, 
        Department department, 
        ICollection<Group> groups)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(firstName) ? TEACHER_FIRST_NAME_EMPTY_KEY : string.Empty,
            string.IsNullOrWhiteSpace(lastName) ? TEACHER_LAST_NAME_EMPTY_KEY : string.Empty,
            firstName.Length > MAX_NAME_LENGTH ? $"{TEACHER_FIRST_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}" : string.Empty,
            lastName.Length > MAX_NAME_LENGTH ? $"{TEACHER_LAST_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}" : string.Empty,
            departmentId == Guid.Empty ? TEACHER_DEPARTMENT_ID_EMPTY_KEY : string.Empty,
            (groups == null || !groups.Any()) ? TEACHER_GROUPS_EMPTY_KEY : string.Empty
        }.Where(e => !string.IsNullOrEmpty(e)).ToList();

        return errors.Count > 0
            ? (null, string.Join("\n", errors))
            : (new Teacher(id, firstName, lastName, departmentId, department, (groups ?? []).ToList().AsReadOnly()), string.Empty);
    }
}
