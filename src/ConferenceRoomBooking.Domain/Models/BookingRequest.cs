using System;

namespace ConferenceRoomBooking.Domain.Models
{
    public class BookingRequest
    {
        public string EmployeeId { get; set; }
        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingRequest(string employeeId, string roomName, DateTime startTime, DateTime endTime)
        {
            EmployeeId = employeeId;
            RoomName = roomName;
            StartTime = startTime;
            EndTime = endTime;
        }

        // Parameterless constructor for serialization
        public BookingRequest()
        {
        }
    }
}
