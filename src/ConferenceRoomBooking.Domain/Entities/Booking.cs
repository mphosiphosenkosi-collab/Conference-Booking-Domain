using System;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public ConferenceRoom ConferenceRoom { get; private set; } = null!;
    public string BookedBy { get; private set; } = string.Empty;
    public string BookerEmail { get; private set; } = string.Empty;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public BookingStatus Status { get; private set; }
    public int NumberOfAttendees { get; private set; }
    public string? MeetingTitle { get; private set; }
    
    public Booking(
        ConferenceRoom conferenceRoom,
        string bookedBy,
        string bookerEmail,
        DateTime startTime,
        DateTime endTime,
        int numberOfAttendees,
        string? meetingTitle = null)
    {
        ConferenceRoom = conferenceRoom;
        BookedBy = bookedBy;
        BookerEmail = bookerEmail;
        StartTime = startTime;
        EndTime = endTime;
        NumberOfAttendees = numberOfAttendees;
        MeetingTitle = meetingTitle;
        Status = BookingStatus.Pending;
    }
    
    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }
}
