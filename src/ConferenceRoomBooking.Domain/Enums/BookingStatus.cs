namespace ConferenceRoomBooking.Domain.Enums;

/// <summary>
/// Represents the status of a booking throughout its lifecycle
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking has been submitted but not yet confirmed
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// Booking has been approved and is active
    /// </summary>
    Confirmed = 2,
    
    /// <summary>
    /// Booking was cancelled by the user or system
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Booking has been completed (past its end time)
    /// </summary>
    Completed = 4
}

