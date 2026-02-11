namespace ConferenceRoomBooking.Domain.Exceptions;

public class NoConferenceRoomsException : Exception
{
    public NoConferenceRoomsException(string message) : base(message) { }
}