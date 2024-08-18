using System.Text.Json.Serialization;

namespace Academy.Server.Data.Models;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? FacultyId { get; set; }
    public Faculty Faculty { get; set; }
    public Guid? TeacherId { get; set; }
    public Teacher Teacher { get; set; }
    [JsonIgnore]
    public ICollection<Student> Students { get; set; }
}
