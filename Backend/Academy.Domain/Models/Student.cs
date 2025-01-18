namespace Academy.Domain.Models;

public class Student
{
    public const int MAX_NAME_LENGTH = 50;

    public const string STUDENT_FIRST_NAME_EMPTY_KEY = "StudentFirstNameEmpty";
    public const string STUDENT_LAST_NAME_EMPTY_KEY = "StudentLastNameEmpty";
    public const string STUDENT_FIRST_NAME_LENGTH_KEY = "StudentFirstNameLength";
    public const string STUDENT_LAST_NAME_LENGTH_KEY = "StudentLastNameLength";

    public Student(Guid id, string firstName, string lastName, Guid groupId, Group group)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        GroupId = groupId;
        Group = group;
    }

    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Guid GroupId { get; }
    public Group Group { get; }

    public static (Student? Student, string Errors) Create(
        Guid id, 
        string firstName, 
        string lastName, 
        Guid groupId, 
        Group group)
    {
        var errors = new List<string>
        {
            string.IsNullOrWhiteSpace(firstName) ? STUDENT_FIRST_NAME_EMPTY_KEY : string.Empty,
            string.IsNullOrWhiteSpace(lastName) ? STUDENT_LAST_NAME_EMPTY_KEY : string.Empty,
            firstName.Length > MAX_NAME_LENGTH ? $"{STUDENT_FIRST_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}" : string.Empty,
            lastName.Length > MAX_NAME_LENGTH ? $"{STUDENT_LAST_NAME_LENGTH_KEY}:{MAX_NAME_LENGTH}" : string.Empty
        }.Where(e => !string.IsNullOrEmpty(e)).ToList();

        return errors.Count > 0
            ? (null, string.Join("\n", errors))
            : (new Student(id, firstName, lastName, groupId, group), string.Empty);
    }
}