using System.Text.Json.Serialization;

namespace Academy.Server.Data.Models;

public class Department
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public ICollection<Teacher> Teachers { get; set; }
}
