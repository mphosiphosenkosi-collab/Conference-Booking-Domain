// src/components/BookingCard/BookingList.jsx
import BookingCard from './BookingCard';
import './BookingList.css';

function BookingList({ bookings, onDeleteBooking }) {
  // Calculate stats for the header
  const totalBookings = bookings.length;
  const activeBookings = bookings.filter(b => b.status === 'confirmed').length;
  const pendingBookings = bookings.filter(b => b.status === 'pending').length;
  
  return (
    <div className="booking-list">
      {/* Modern Header with Stats */}
      <div className="booking-list-header">
        <div className="header-left">
          <h1>
            <span className="header-icon"></span>
            Bookings Overview
          </h1>
          <p className="header-subtitle">Manage and view all conference room bookings</p>
        </div>
        
        <div className="header-stats">
          <div className="stat-badge total">
            <span className="stat-number">{totalBookings}</span>
            <span className="stat-label">Total</span>
          </div>
          <div className="stat-badge active">
            <span className="stat-number">{activeBookings}</span>
            <span className="stat-label">Active</span>
          </div>
          <div className="stat-badge pending">
            <span className="stat-number">{pendingBookings}</span>
            <span className="stat-label">Pending</span>
          </div>
        </div>
      </div>

      {/* Bookings Grid */}
      {bookings.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon"></div>
          <h3>No Bookings Yet</h3>
          <p>Create your first booking using the form above</p>
        </div>
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
              onDelete={onDeleteBooking}  // Pass down the delete handler extra credit
            />
          ))}
        </div>
      )}
    </div>
  );
}

export default BookingList;