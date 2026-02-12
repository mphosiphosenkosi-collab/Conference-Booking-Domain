using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Domain.Enums;

namespace ConferenceRoomBooking.Logic
{
    public class BookingManager
    {
        private readonly List<Booking> _bookings = new();
        private int _nextId = 1;

        public Booking CreateBooking(
            int roomId,
            string userEmail,
            DateTime start,
            DateTime end)
        {
            // basic validation
            if (start >= end)
                throw new ArgumentException("Start must be before end");

            // conflict check
            var conflict = _bookings.Any(b =>
                b.RoomId == roomId &&
                b.Status != BookingStatus.Cancelled &&
                start < b.EndTime &&
                end > b.StartTime);

            if (conflict)
                throw new InvalidOperationException("Booking conflict detected");

            // ✅ MATCHES YOUR BOOKING CONSTRUCTOR
            var booking = new Booking
            {
                Id = _nextId++,
                RoomId = roomId,
                UserEmail = userEmail,
                StartTime = start,
                EndTime = end
            };

            _bookings.Add(booking);

            return booking;
        }

        public IEnumerable<Booking> GetAll()
        {
            return _bookings;
        }

        public Booking? GetById(int id)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }

        public void Cancel(int id)
        {
            var booking = GetById(id)
                ?? throw new InvalidOperationException("Booking not found");

            booking.Cancel();
        }

        public void Confirm(int id)
        {
            var booking = GetById(id)
                ?? throw new InvalidOperationException("Booking not found");

            booking.Confirm();
        }

        public void Complete(int id)
        {
            var booking = GetById(id)
                ?? throw new InvalidOperationException("Booking not found");

            booking.Complete();
        }
    }
}
