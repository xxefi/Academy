using System.Text.Json.Serialization;

namespace Academy.Server.Data.Models;

public class Student
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid GroupId { get; set; }
    [JsonIgnore]
    public Group Groups { get; set; }
}
