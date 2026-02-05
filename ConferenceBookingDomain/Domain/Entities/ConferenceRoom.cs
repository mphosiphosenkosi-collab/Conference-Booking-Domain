namespace ConferenceRoomBooking.Domain.Entities;

public class ConferenceRoom
{
    public int Id { get; }
    public string Name { get; }
    public int Capacity { get; }
    public string RoomType { get; }

    public ConferenceRoom(int id, string name, int capacity, string roomType)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Capacity = capacity;
        RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
    }
    
    // Private constructor for JSON deserialization
    private ConferenceRoom() { }
}
