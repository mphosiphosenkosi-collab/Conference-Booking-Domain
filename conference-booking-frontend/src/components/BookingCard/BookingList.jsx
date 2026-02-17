// src/components/BookingCard/BookingList.jsx
import BookingCard from './BookingCard';

function BookingList({ bookings }) {  // receives props from App.jsx - Assign 1.2
  return (
    <div>
      <h2>Current Bookings</h2>
      {bookings.length === 0 ? (
        <p>No bookings yet. Add your first booking above!</p>
      ) : (
        <div className="bookings-grid">
          {bookings.map(booking => (
            <BookingCard
              key={booking.id}
              id={booking.id}
              roomName={booking.roomName}
              date={booking.date}
              startTime={booking.startTime}
              endTime={booking.endTime}
              userName={booking.userName}
              status={booking.status}
            />
          ))}
        </div>
      )}
    </div>
  );
}

export default BookingList;