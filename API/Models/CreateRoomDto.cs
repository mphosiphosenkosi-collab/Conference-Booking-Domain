namespace API.Models;

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public List<string> Features { get; set; } = new();
}
