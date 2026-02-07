namespace API.Models
{
    public class ConferenceRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public List<string> Features { get; set; } = new();
    }

    public class CreateRoomDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public List<string> Features { get; set; } = new();
    }
}