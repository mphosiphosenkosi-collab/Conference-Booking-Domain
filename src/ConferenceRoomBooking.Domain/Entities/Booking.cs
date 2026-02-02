using System;

namespace ConferenceRoomBooking.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public ConferenceRoom Room { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Enums.BookingStatus Status { get; set; }

        public Booking(
            int id,
            string employeeId,
            ConferenceRoom room,
            DateTime startTime,
            DateTime endTime,
            Enums.BookingStatus status)
        {
            Id = id;
            EmployeeId = employeeId;
            Room = room;
            StartTime = startTime;
            EndTime = endTime;
            Status = status;
        }

        // Parameterless constructor for EF Core
        public Booking()
        {
        }
    }
}
