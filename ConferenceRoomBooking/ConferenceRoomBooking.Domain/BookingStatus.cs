namespace ConferenceRoomBooking.Domain.Enums
{
    /// <summary>
    /// Represents the status of a booking request
    /// Based on real-world booking lifecycle
    /// </summary>
    public enum BookingStatus
    {
        Pending,    // Submitted, awaiting confirmation
        Confirmed,  // Approved and scheduled
        Cancelled,  // Cancelled by user
        Completed   // Meeting has taken place
    }
}