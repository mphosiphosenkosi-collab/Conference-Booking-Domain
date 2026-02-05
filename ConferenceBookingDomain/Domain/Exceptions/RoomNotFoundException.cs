namespace ConferenceRoomBooking.Domain.Exceptions;

public class RoomNotFoundException : Exception
{
    public string RoomName { get; }

    public RoomNotFoundException(string roomName)
        : base($"Room '{roomName}' not found")
    {
        RoomName = roomName;
    }

    public RoomNotFoundException(string roomName, Exception innerException)
        : base($"Room '{roomName}' not found", innerException)
    {
        RoomName = roomName;
    }
}
