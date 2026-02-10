namespace API.Models;

public class ConferenceRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public IEnumerable<string> Features { get; set; } = new List<string>();
}
