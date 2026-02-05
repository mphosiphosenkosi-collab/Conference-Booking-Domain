using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Persistence.Stores; // your JSON store namespace

namespace ConferenceRoomBooking.Logic.Services
{
    public class BookingManager
    {
        private readonly BookingFileStore _store;

        public BookingManager(BookingFileStore store)
        {
            _store = store;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _store.LoadAsync();
        }

        // THIS IS THE MISSING PIECE
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var allBookings = await _store.LoadAsync();

            // Optional: check for overlapping bookings, etc.
            allBookings.Add(booking);

            await _store.SaveAsync(allBookings);

            return booking;
        }
    }
}
