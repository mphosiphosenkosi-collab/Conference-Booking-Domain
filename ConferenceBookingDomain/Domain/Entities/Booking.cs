using System.Text.Json.Serialization;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities;

public class Booking
{
    public int Id { get; private set; }                   // DB-generated
    public string EmployeeId { get; private set; }        // Can be null if not required yet
    public int RoomId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // ✅ Minimal constructor for API creation (calculator pattern)
    public Booking(int roomId, DateTime startTime, DateTime endTime, string employeeId = null)
    {
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
        EmployeeId = employeeId;
        Status = BookingStatus.Pending;   // default for new booking
        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // ✅ Full constructor for deserialization / DB reads
    [JsonConstructor]
    public Booking(int id, string employeeId, int roomId,
                   DateTime startTime, DateTime endTime, BookingStatus status, DateTime createdAt)
    {
        Id = id;
        EmployeeId = employeeId;
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        CreatedAt = createdAt;

        Validate();
    }

    private void Validate()
    {
        if (StartTime >= EndTime)
            throw new ArgumentException("Start time must be before end time");
        if (StartTime < DateTime.UtcNow.AddMinutes(-5))
            throw new ArgumentException("Start time cannot be in the past");
    }

    public void Cancel()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException($"Cannot cancel booking with status {Status}");
        Status = BookingStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm booking with status {Status}");
        Status = BookingStatus.Confirmed;
    }
}
