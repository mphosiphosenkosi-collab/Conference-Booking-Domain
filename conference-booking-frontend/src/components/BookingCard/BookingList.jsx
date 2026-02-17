import BookingCard from './BookingCard';
import mockBookings from '../../data/mockData';
import './BookingList.css';

function BookingList() {
  // Handler functions moved here
  const handleEdit = (bookingId) => {
    alert(`Edit booking #${bookingId}`);
  };
  
  const handleCancel = (bookingId) => {
    alert(`Cancel booking #${bookingId}`);
  };
  
  return (
    <div className="booking-list">
      <h1>Current Bookings</h1>
      
      <div className="bookings-grid">
        {mockBookings.map(booking => (
          <BookingCard
            key={booking.id}
            id={booking.id}
            roomName={booking.roomName}
            date={booking.date}
            startTime={booking.startTime}
            endTime={booking.endTime}
            userName={booking.userName}
            status={booking.status}
            onEdit={handleEdit}
            onCancel={handleCancel}
          />
        ))}
      </div>
    </div>
  );
}

export default BookingList;