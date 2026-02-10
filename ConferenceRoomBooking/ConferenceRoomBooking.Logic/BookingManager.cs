using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Logic.Persistence;

namespace ConferenceRoomBooking.Logic
{
    /// <summary>
    /// BOOKING MANAGER = BUSINESS RULES LAYER
    /// This class decides WHAT is allowed.
    /// It does NOT know how data is stored.
    /// </summary>
    public class BookingManager
    {
        private readonly List<ConferenceRoom> _rooms = new();
        private readonly List<Booking> _bookings = new();

        // 🔗 persistence injected — not hardcoded
        private readonly IBookingStore _store;

        public BookingManager(IBookingStore store)
        {
            _store = store;
        }

        // ===============================-
        // ROOM METHODS
        // ===============================

        public void AddRoom(ConferenceRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (_rooms.Any(r => r.Id == room.Id))
                throw new ArgumentException("Room already exists");

            _rooms.Add(room);
        }

        public IReadOnlyList<ConferenceRoom> GetRooms()
            => _rooms.ToList();


        // ===============================
        // BOOKING METHODS
        // ===============================

        public IReadOnlyList<Booking> GetBookings()
            => _bookings.ToList();


        /// <summary>
        /// Create booking with full validation
        /// </summary>
        public Booking CreateBooking(
            int id,
            int roomId,
            string userEmail,
            DateTime start,
            DateTime end)
        {
            // ✅ Validate time
            if (start >= end)
                throw new ArgumentException("Invalid time range");

            // ✅ Room must exist
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
                throw new ArgumentException("Room does not exist");

            // ✅ Check overlap (Emily logic — adapted)
            bool overlaps = _bookings.Any(b =>
                b.RoomId == roomId &&
                start < b.EndTime &&
                end > b.StartTime);

            if (overlaps)
                throw new BookingConflictException("Time slot already booked");

            // ✅ Create booking
            var booking = new Booking(id, roomId, userEmail, start, end);

            _bookings.Add(booking);

            return booking;
        }

        public async Task InitializeAsync()
        {
            var loaded = await _store.LoadAsync();
            _bookings.Clear();
            _bookings.AddRange(loaded);
        }

        private async Task PersistAsync()
        {
            await _store.SaveAsync(_bookings);
        }



        /// <summary>
        /// Cancel booking by id
        /// </summary>
        public bool CancelBooking(int bookingId)
        {
            var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
                return false;

            _bookings.Remove(booking);
            return true;
        }


        // ===============================
        // PERSISTENCE METHODS
        // ===============================

        public async Task SaveAsync()
        {
            await _store.SaveAsync(_bookings.ToList());
        }

        public async Task LoadAsync()
        {
            var loaded = await _store.LoadAsync();

            _bookings.Clear();
            _bookings.AddRange(loaded);
        }
    }
}
