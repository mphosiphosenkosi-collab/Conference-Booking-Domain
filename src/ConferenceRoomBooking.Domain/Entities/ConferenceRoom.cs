namespace ConferenceRoomBooking.Domain.Entities
{
    public class ConferenceRoom
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string RoomType { get; set; }

        // Comment out extra fields for now, do NOT delete
        // public bool HasProjector { get; set; }
        // public bool HasWhiteboard { get; set; }
        // public string Location { get; set; }

        public ConferenceRoom(string name, int capacity, string roomType)
        {
            Name = name;
            Capacity = capacity;
            RoomType = roomType;
        }
        
        // Parameterless constructor for EF Core
        public ConferenceRoom()
        {
        }
    }
}
