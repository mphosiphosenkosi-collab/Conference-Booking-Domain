import Button from '../Button/Button';  // Go up one level, then into Button folder
import './BookingCard.css';

function BookingCard({ 
  id,
  roomName,
  date,
  startTime,
  endTime,
  userName,
  status,
  onEdit,
  onCancel
}) {
  
  // Helper function to get status color (still needed for dynamic style)
  const getStatusColor = (status) => {
    switch(status?.toLowerCase()) {
      case 'confirmed':
        return '#27ae60';  // green
      case 'pending':
        return '#f39c12';  // orange
      case 'cancelled':
        return '#e74c3c';  // red
      default:
        return '#7f8c8d';  // gray
    }
  };
  
  // Get status icon
  const getStatusIcon = (status) => {
    switch(status?.toLowerCase()) {
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
  
  return (
    <div className="booking-card">
      {/* Header with room name and status */}
      <div className="booking-card-header">
        <h3 className="booking-card-room">{roomName}</h3>
        <span 
          className="booking-card-status"
          style={{ backgroundColor: getStatusColor(status) }}  // Only dynamic part!
        >
          {getStatusIcon(status)} {status}
        </span>
      </div>
      
      {/* Booking Details */}
      <div className="booking-card-details">
        <div className="booking-card-detail-row">
          <span className="booking-card-icon"></span>
          <span className="booking-card-label">Date:</span>
          <span>{date}</span>
        </div>
        <div className="booking-card-detail-row">
          <span className="booking-card-icon"></span>
          <span className="booking-card-label">Time:</span>
          <span>{startTime} - {endTime}</span>
        </div>
        <div className="booking-card-detail-row">
          <span className="booking-card-icon"></span>
          <span className="booking-card-label">Booked by:</span>
          <span>{userName}</span>
        </div>
        <div className="booking-card-detail-row">
          <span className="booking-card-icon"></span>
          <span className="booking-card-label">Booking ID:</span>
          <span>#{id}</span>
        </div>
      </div>
      
      {/* Action Buttons */}
      <div className="booking-card-actions">
        <Button 
          variant="secondary" 
          size="small" 
          onClick={() => onEdit?.(id)}
        >
          ✏️ Edit
        </Button>
        <Button 
          variant="danger" 
          size="small" 
          onClick={() => onCancel?.(id)}
        >
          ❌ Cancel
        </Button>
      </div>
    </div>
  );
}

export default BookingCard;