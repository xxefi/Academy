namespace Academy.Domain.Models;
public class Group
{
    public const int MAX_NAME_LENGTH = 100;

    public const string GROUP_NAME_EMPTY_KEY = "GroupNameEmpty";
    public const string GROUP_NAME_LENGTH_KEY = "GroupNameLength";
    public const string GROUP_STUDENTS_EMPTY_KEY = "GroupStudentsEmpty";

    public Group(Guid id, string name, Guid? facultyId, Faculty faculty, Guid? teacherId, Teacher teacher, ICollection<Student> students, ICollection<Teacher> teachers)
    {
        Id = id;
        Name = name;
        FacultyId = facultyId;
        Faculty = faculty;
        TeacherId = teacherId;
        Teacher = teacher;
    }

    public Guid Id { get; }
    public string Name { get; }
    public Guid? FacultyId { get; }
    public Faculty Faculty { get; }
    public Guid? TeacherId { get; }
    public Teacher Teacher { get; }
    public ICollection<Student> Students { get; }
    public ICollection<Teacher> Teachers { get; }

    public static (Group? Group, string Errors) Create(
        Guid id, 
        string name, 
        Guid? facultyId, 
        Faculty faculty,
        Guid? teacherId, 
        Teacher teacher, 
        ICollection<Student> students, 
        ICollection<Teacher> teachers)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(name) ? GROUP_NAME_EMPTY_KEY : string.Empty,
            name.Length > MAX_NAME_LENGTH ? $"{GROUP_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}" : string.Empty,
            (students == null || !students.Any()) ? GROUP_STUDENTS_EMPTY_KEY : string.Empty
        }.Where(e => !string.IsNullOrEmpty(e)).ToList();

        return errors.Count > 0
            ? (null, string.Join("\n", errors))
            : (new Group(id, name, facultyId, faculty, teacherId, teacher, (students ?? []).ToList().AsReadOnly(), (teachers ?? []).ToList().AsReadOnly()), string.Empty);
    }
}