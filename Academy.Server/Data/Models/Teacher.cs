using System.Text.Json.Serialization;

namespace Academy.Server.Data.Models;

public class Teacher
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid DepartmentId { get; set; }
    [JsonIgnore]
    public Department Department { get; set; }
    [JsonIgnore]
    public ICollection<Group> Groups { get; set; }
}
