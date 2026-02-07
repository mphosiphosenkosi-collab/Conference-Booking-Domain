namespace ConferenceRoomBooking.Domain
{
    public class BookingConflictException : Exception
    {
        public BookingConflictException() { }

        public BookingConflictException(string message) 
            : base(message) { }

        public BookingConflictException(string message, Exception inner) 
            : base(message, inner) { }
    }
}