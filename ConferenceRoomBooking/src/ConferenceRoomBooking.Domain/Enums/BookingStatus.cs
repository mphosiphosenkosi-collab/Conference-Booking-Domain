namespace ConferenceRoomBooking.Domain.Enums
{
    /// <summary>
    /// Represents the status of a booking
    /// </summary>
    public enum BookingStatus
    {
        /// <summary>
        /// Booking has been requested but not yet confirmed
        /// </summary>
        Pending = 1,
        
        /// <summary>
        /// Booking has been confirmed
        /// </summary>
        Confirmed = 2,
        
        /// <summary>
        /// Booking has been cancelled
        /// </summary>
        Cancelled = 3,
        
        /// <summary>
        /// Booking has been completed
        /// </summary>
        Completed = 4
    }
}
