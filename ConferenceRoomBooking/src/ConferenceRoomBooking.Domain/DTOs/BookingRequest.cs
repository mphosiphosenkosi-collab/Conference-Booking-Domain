namespace ConferenceRoomBooking.Domain.DTOs;

public class BookingRequest
{
    public required string EmployeeId { get; set; }
    public required string RoomName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
