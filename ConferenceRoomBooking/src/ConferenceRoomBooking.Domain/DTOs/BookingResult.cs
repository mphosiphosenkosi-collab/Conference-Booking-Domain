using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Domain.DTOs;

public class BookingResult
{
    public Booking Booking { get; }
    public bool Success { get; }
    public string Message { get; }
    public List<string> Errors { get; }

    public BookingResult(Booking booking, bool success, string message = "")
    {
        Booking = booking;
        Success = success;
        Message = message;
        Errors = new List<string>();
    }

    public BookingResult(bool success, string message, List<string> errors)
    {
        Booking = null!;
        Success = success;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static BookingResult SuccessResult(Booking booking, string message = "Booking created successfully")
        => new(booking, true, message);

    public static BookingResult FailureResult(string message, List<string> errors = null)
        => new(false, message, errors ?? new List<string>());
}
