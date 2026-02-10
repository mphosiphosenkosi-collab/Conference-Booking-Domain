using ConferenceRoomBooking.Domain;

namespace ConferenceRoomBooking.Logic.Persistence;

public interface IBookingStore
{
    Task SaveAsync(IEnumerable<Booking> bookings);
    Task<List<Booking>> LoadAsync();
}
