using System.Text.Json.Serialization;

namespace Academy.Server.Data.Models;

public class Faculty
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public ICollection<Group> Groups { get; set; }
}
