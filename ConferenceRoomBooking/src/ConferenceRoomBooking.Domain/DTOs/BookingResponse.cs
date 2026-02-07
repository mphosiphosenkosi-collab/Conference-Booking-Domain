namespace ConferenceRoomBooking.Domain.DTOs;

public class BookingResponse
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public string BookedBy { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
    // Add other properties as needed
}