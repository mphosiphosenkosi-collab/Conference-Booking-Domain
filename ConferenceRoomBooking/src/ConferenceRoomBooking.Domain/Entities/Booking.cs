using System.Text.Json.Serialization;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities;

public class Booking
{
    public int Id { get; }
    public string EmployeeId { get; }  // Removed 'required'
    public int RoomId { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAt { get; }

    // Main constructor for business logic
    public Booking(int id, string employeeId, int roomId, 
                  DateTime startTime, DateTime endTime, BookingStatus status)
    {
        Id = id;
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        
        Validate();
    }

    // JsonConstructor for serialization (calculator pattern)
    [JsonConstructor]
    public Booking(int id, string employeeId, int roomId, 
                  DateTime startTime, DateTime endTime, BookingStatus status, DateTime createdAt)
    {
        Id = id;
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        RoomId = roomId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        CreatedAt = createdAt;
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
