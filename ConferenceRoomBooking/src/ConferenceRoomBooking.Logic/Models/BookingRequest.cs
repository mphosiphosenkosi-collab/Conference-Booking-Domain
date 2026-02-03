namespace ConferenceRoomBooking.Services.Models
{
    /// <summary>
    /// Represents a request to create a new booking
    /// </summary>
    public class BookingRequest
    {
        public string EmplyoyeeId { get; set; }
        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingRequest(string employeeId, string roomName, DateTime startTime, DateTime endTime)
        {
            EmplyoyeeId = employeeId;
            RoomName = roomName;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
