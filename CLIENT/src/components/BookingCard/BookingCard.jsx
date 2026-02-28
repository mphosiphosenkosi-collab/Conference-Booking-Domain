// src/components/BookingCard/BookingCard.jsx
import { useState } from 'react';
import './BookingCard.css';

// Room images mapping
const roomImages = {
  'A': 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=400&h=200&fit=crop',
  'B': 'https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=400&h=200&fit=crop',
  'C': 'https://images.unsplash.com/photo-1497215842964-222b430dc094?w=400&h=200&fit=crop',
  'D': 'https://images.unsplash.com/photo-1504384308090-c894fdcc538d?w=400&h=200&fit=crop'
};

// Default image if room not found
const defaultImage = 'https://images.unsplash.com/photo-1497366216548-37526070297c?w=400&h=200&fit=crop';

// Navbar gradient colors
const gradients = {
  confirmed: 'linear-gradient(135deg, #2c3e50 0%, #3498db 100%)',
  pending: 'linear-gradient(135deg, #f39c12 0%, #f1c40f 100%)',
  cancelled: 'linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)',
  default: 'linear-gradient(135deg, #2c3e50 0%, #1a252f 100%)'
};

const BookingCard = ({ booking, onDelete }) => {
  const [isDeleting, setIsDeleting] = useState(false);
  const [imageError, setImageError] = useState(false);

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this booking?')) {
      setIsDeleting(true);
      await onDelete(booking.id);
      setIsDeleting(false);
    }
  };

  const handleImageError = () => {
    setImageError(true);
  };

  const gradient = gradients[booking.status] || gradients.default;
  const roomImage = !imageError ? (roomImages[booking.room] || defaultImage) : defaultImage;

  // Format date nicely
  const formatDate = (dateString) => {
    const options = { 
      weekday: 'short', 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    };
    return new Date(dateString).toLocaleDateString('en-US', options);
  };

  return (
    <div className={`booking-tile ${isDeleting ? 'deleting' : ''}`}>
      {/* Card Image */}
      <div className="tile-image-container">
        <img 
          src={roomImage}
          alt={`Room ${booking.room}`}
          className="tile-image"
          onError={handleImageError}
          loading="lazy"
        />
        <div className="tile-overlay" style={{ background: gradient }}></div>
        <div className="tile-room-badge">
          Room {booking.room}
        </div>
        <div className="tile-id-badge">
          ID: {booking.id}
        </div>
      </div>

      {/* Card Content */}
      <div className="tile-content">
        <div className="tile-header">
          <h3 className="tile-title">{booking.conferenceName}</h3>
          <span className="tile-category">
            {booking.category === 'internal' ? '🏢 Internal' : '🤝 Client'}
          </span>
        </div>

        <div className="tile-details-grid">
          {/* Date */}
          <div className="tile-detail-item">
            <span className="tile-detail-icon">📅</span>
            <div className="tile-detail-info">
              <span className="tile-detail-label">Date</span>
              <span className="tile-detail-value">{formatDate(booking.date)}</span>
            </div>
          </div>

          {/* Attendees */}
          <div className="tile-detail-item">
            <span className="tile-detail-icon">👥</span>
            <div className="tile-detail-info">
              <span className="tile-detail-label">Attendees</span>
              <span className="tile-detail-value">{booking.attendees} people</span>
            </div>
          </div>

          {/* Status */}
          <div className="tile-detail-item tile-detail-full">
            <span className="tile-detail-icon">📊</span>
            <div className="tile-detail-info">
              <span className="tile-detail-label">Status</span>
              <span 
                className="tile-status-text"
                style={{ 
                  background: gradient,
                  color: 'white',
                  padding: '4px 12px',
                  borderRadius: '20px',
                  display: 'inline-block',
                  fontSize: '0.8rem',
                  fontWeight: '600',
                  textTransform: 'uppercase'
                }}
              >
                {booking.status}
              </span>
            </div>
          </div>

          {/* Room (duplicate for completeness) */}
          <div className="tile-detail-item">
            <span className="tile-detail-icon">📍</span>
            <div className="tile-detail-info">
              <span className="tile-detail-label">Location</span>
              <span className="tile-detail-value">Conference Room {booking.room}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Card Footer */}
      <div className="tile-footer">
        <button
          onClick={handleDelete}
          className="tile-delete-button"
          disabled={isDeleting}
        >
          {isDeleting ? (
            <>
              <span className="spinner-small"></span>
              Deleting...
            </>
          ) : (
            <>
              <span className="delete-icon">🗑️</span>
              Cancel Booking
            </>
          )}
        </button>
      </div>
    </div>
  );
};

export default BookingCard;