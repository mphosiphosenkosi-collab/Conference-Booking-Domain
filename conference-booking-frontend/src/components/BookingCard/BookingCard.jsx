import Button from '../Button/Button';
// src/components/BookingCard/BookingCard.jsx
import './BookingCard.css';

function BookingCard({
  id,
  roomName,
  date,
  startTime,
  endTime,
  userName,
  status,
  onDelete
}) {

  const getStatusColor = (status) => {
    switch (status?.toLowerCase()) {
      case 'confirmed':
        return '#27ae60';
      case 'pending':
        return '#f39c12';
      case 'cancelled':
        return '#e74c3c';
      default:
        return '#7f8c8d';
    }
  };

  const getStatusIcon = (status) => {
    switch (status?.toLowerCase()) {
      case 'confirmed':
        return '✅';
      case 'pending':
        return '⏳';
      case 'cancelled':
        return '❌';
      default:
        return '📋';
    }
  };

  const handleDelete = () => {
    if (window.confirm(`Cancel booking for ${roomName}?`)) {
      onDelete(id);
    }
  };

  return (
    <div className={`booking-card ${status?.toLowerCase()}`}>
      {/* Status Badge */}
      <div
        className="status-badge"
        style={{ backgroundColor: getStatusColor(status) }}
      >
        {getStatusIcon(status)} {status}
      </div>

      {/* Room Name */}
      <h3>{roomName}</h3>

      {/* Details */}
      <div className="booking-details">
        <p>
          <strong> Date:</strong> {date}
        </p>
        <p>
          <strong> Time:</strong> {startTime} - {endTime}
        </p>
        <p>
          <strong> Booked by:</strong> {userName}
        </p>
        <p className="booking-id">
          <strong> ID:</strong> #{id}
        </p>
      </div>

      {/* Primary Blue Button */}
      <button className="btn-action-primary">
        <span></span> Edit
      </button>

      {/* Danger Red Button */}
      <button className="btn-action-danger" onClick={handleDelete}>
        <span></span> Cancel
      </button>
    </div>
  );
}

export default BookingCard;