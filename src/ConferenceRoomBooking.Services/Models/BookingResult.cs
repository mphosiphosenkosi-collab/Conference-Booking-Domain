namespace ConferenceRoomBooking.Services.Models
{
    /// <summary>
    /// Represents the result of a booking operation
    /// </summary>
    public class BookingResult
    {
        /// <summary>
        /// Indicates whether the booking was successful
        /// </summary>
        public bool IsSuccess { get; }
        
        /// <summary>
        /// The created booking if successful
        /// </summary>
        public Domain.Entities.Booking? Booking { get; }
        
        /// <summary>
        /// Error message if the booking failed
        /// </summary>
        public string ErrorMessage { get; }
        
        /// <summary>
        /// Creates a successful booking result
        /// </summary>
        public BookingResult(Domain.Entities.Booking booking)
        {
            IsSuccess = true;
            Booking = booking;
            ErrorMessage = string.Empty;
        }
        
        /// <summary>
        /// Creates a failed booking result
        /// </summary>
        public BookingResult(string errorMessage)
        {
            IsSuccess = false;
            Booking = null;
            ErrorMessage = errorMessage;
        }
        
        /// <summary>
        /// Helper method to create successful result
        /// </summary>
        public static BookingResult Success(Domain.Entities.Booking booking) => new(booking);
        
        /// <summary>
        /// Helper method to create failed result
        /// </summary>
        public static BookingResult Failure(string errorMessage) => new(errorMessage);
    }
}
