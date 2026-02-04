using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.DTOs;

public class BookingResult
{
    public Booking? Booking { get; }
    public bool Success { get; }
    public string Message { get; }
    public List<string> Errors { get; }
    public string ErrorCode { get; } // For domain-specific error codes

    public BookingResult(Booking? booking, bool success, string message = "")
    {
        Booking = booking;
        Success = success;
        Message = message;
        Errors = new List<string>();
        ErrorCode = string.Empty;
    }

    public BookingResult(bool success, string message, string errorCode = "", List<string>? errors = null)
    {
        Booking = null;
        Success = success;
        Message = message;
        ErrorCode = errorCode;
        Errors = errors ?? new List<string>();
    }

    public static BookingResult SuccessResult(Booking booking, string message = "Booking created successfully")
        => new(booking, true, message);

    public static BookingResult FailureResult(string message, string errorCode = "", List<string>? errors = null)
        => new(false, message, errorCode, errors);

    // Domain-specific failure results
    public static BookingResult RoomUnavailable(string roomName, DateTime startTime, DateTime endTime)
        => new(false, $"Room '{roomName}' is not available from {startTime} to {endTime}", "ROOM_UNAVAILABLE");

    public static BookingResult InvalidTimeRange()
        => new(false, "Start time must be before end time", "INVALID_TIME_RANGE");

    public static BookingResult PastStartTime()
        => new(false, "Start time cannot be in the past", "PAST_START_TIME");
}
