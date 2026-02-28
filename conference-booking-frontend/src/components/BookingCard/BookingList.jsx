// src/components/BookingCard/BookingList.jsx
import BookingCard from './BookingCard';
import './BookingList.css';

const BookingList = ({ bookings, onDelete }) => {
  if (bookings.length === 0) {
    return (
      <div className="empty-state">
        <div className="empty-icon">📭</div>
        <h3>No Bookings Found</h3>
        <p>Try adjusting your filters or create a new booking</p>
      </div>
    );
  }

  return (
    <div className="booking-list">
      <div className="tile-grid">
        {bookings.map(booking => (
          <BookingCard
            key={booking.id}
            booking={booking}
            onDelete={onDelete}
          />
        ))}
      </div>
    </div>
  );
};

export default BookingList;