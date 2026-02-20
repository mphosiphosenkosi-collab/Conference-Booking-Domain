// src/components/BookingForm/BookingForm.jsx
import { useState } from 'react';
import './BookingForm.css';

function BookingForm({ onAdd }) {
  const [isOpen, setIsOpen] = useState(false);
  const [roomName, setRoomName] = useState('');
  const [userName, setUserName] = useState('');
  const [date, setDate] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  
  // NEW: State for validation errors
  const [errors, setErrors] = useState({});

  // Available rooms for dropdown
  const availableRooms = [
    'Meeting Room 1',
    'Meeting Room 2',
    'Meeting Room 3',
    'Conference Room A',
    'Conference Room B',
    'Board Room'
  ];

  const validateForm = () => {
    const newErrors = {};
    
    if (!roomName.trim()) newErrors.roomName = 'Please select a room';
    if (!userName.trim()) newErrors.userName = 'Your name is required';
    if (!date) newErrors.date = 'Date is required';
    if (!startTime) newErrors.startTime = 'Start time is required';
    if (!endTime) newErrors.endTime = 'End time is required';
    
    // Optional: Check if end time is after start time
    if (startTime && endTime && startTime >= endTime) {
      newErrors.timeRange = 'End time must be after start time';
    }
    
    return newErrors;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Validate form
    const validationErrors = validateForm();
    
    if (Object.keys(validationErrors).length > 0) {
      // Show errors
      setErrors(validationErrors);
      
      // Show alert with first error (user-friendly)
      const firstError = Object.values(validationErrors)[0];
      alert(`⚠️ ${firstError}`);
      return;
    }
    
    // Clear any previous errors
    setErrors({});
    
    // Create new booking
    const newBooking = {
      id: Date.now(),
      roomName: roomName.trim(),
      userName: userName.trim(),
      date,
      startTime,
      endTime,
      status: 'pending'
    };
    
    // Add booking
    onAdd(newBooking);
    
    // Clear form and close modal
    setRoomName('');
    setUserName('');
    setDate('');
    setStartTime('');
    setEndTime('');
    setIsOpen(false);
  };

  // Helper function to check if a field has an error
  const hasError = (fieldName) => !!errors[fieldName];

  // Calculate booking duration if times are selected
  const getBookingDuration = () => {
    if (startTime && endTime) {
      return `${startTime} - ${endTime}`;
    }
    return 'Not set';
  };

  // Format date for display
  const formatDate = (dateString) => {
    if (!dateString) return 'Not set';
    const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  const openModal = () => {
    setIsOpen(true);
    document.body.style.overflow = 'hidden'; // Prevent background scrolling
  };

  const closeModal = () => {
    setIsOpen(false);
    document.body.style.overflow = 'unset'; // Restore scrolling
    // Optional: Reset form when closing
    setRoomName('');
    setUserName('');
    setDate('');
    setStartTime('');
    setEndTime('');
    setErrors({});
  };

  return (
    <>
      {/* Floating Action Button to open the modal */}
      <button className="fab-button" onClick={openModal}>
        <span className="fab-icon">+</span>
        <span className="fab-text">New Booking</span>
      </button>

      {/* Modal Overlay */}
      {isOpen && (
        <div className="modal-overlay" onClick={closeModal}>
          {/* Modal Container - Prevent closing when clicking inside */}
          <div className="modal-container" onClick={(e) => e.stopPropagation()}>
            {/* Close button */}
            <button className="modal-close" onClick={closeModal}>×</button>

            {/* Booking Overview Section */}
            <div className="booking-overview">
              <h3 className="overview-title">Booking Summary</h3>
              <div className="overview-grid">
                <div className="overview-item">
                  <span className="overview-label">Room</span>
                  <span className="overview-value">{roomName || '—'}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Booker</span>
                  <span className="overview-value">{userName || '—'}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Date</span>
                  <span className="overview-value">{formatDate(date)}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Time</span>
                  <span className="overview-value">{getBookingDuration()}</span>
                </div>
              </div>
            </div>

            {/* Booking Form */}
            <div className="booking-form-container">
              <div className="booking-form-header">
                <h2>Create New Booking</h2>
                <p className="form-subtitle">Fill in the details to reserve a meeting room</p>
              </div>
              
              <form onSubmit={handleSubmit}>
                <div className="form-content">
                  {/* Room Name Dropdown */}
                  <div className={`form-group ${hasError('roomName') ? 'has-error' : ''}`}>
                    <label>Select Meeting Room</label>
                    <select
                      value={roomName}
                      onChange={(e) => {
                        setRoomName(e.target.value);
                        if (errors.roomName) {
                          setErrors({...errors, roomName: null});
                        }
                      }}
                      className={hasError('roomName') ? 'error' : ''}
                    >
                      <option value="">— Choose a room —</option>
                      {availableRooms.map(room => (
                        <option key={room} value={room}>{room}</option>
                      ))}
                    </select>
                    {errors.roomName && (
                      <span className="error-message">{errors.roomName}</span>
                    )}
                  </div>
                  
                  {/* User Name Field */}
                  <div className={`form-group ${hasError('userName') ? 'has-error' : ''}`}>
                    <label>Your Name</label>
                    <input
                      type="text"
                      value={userName}
                      onChange={(e) => {
                        setUserName(e.target.value);
                        if (errors.userName) {
                          setErrors({...errors, userName: null});
                        }
                      }}
                      placeholder="e.g., John Smith"
                      className={hasError('userName') ? 'error' : ''}
                    />
                    {errors.userName && (
                      <span className="error-message">{errors.userName}</span>
                    )}
                  </div>
                  
                  {/* Date Field */}
                  <div className={`form-group ${hasError('date') ? 'has-error' : ''}`}>
                    <label>Select Date</label>
                    <input
                      type="date"
                      value={date}
                      onChange={(e) => {
                        setDate(e.target.value);
                        if (errors.date) {
                          setErrors({...errors, date: null});
                        }
                      }}
                      className={hasError('date') ? 'error' : ''}
                      min={new Date().toISOString().split('T')[0]}
                    />
                    {errors.date && (
                      <span className="error-message">{errors.date}</span>
                    )}
                  </div>
                  
                  {/* Time Fields */}
                  <div className="time-row">
                    <div className={`form-group ${hasError('startTime') ? 'has-error' : ''}`}>
                      <label>Start Time</label>
                      <input
                        type="time"
                        value={startTime}
                        onChange={(e) => {
                          setStartTime(e.target.value);
                          if (errors.startTime || errors.timeRange) {
                            setErrors({...errors, startTime: null, timeRange: null});
                          }
                        }}
                        className={hasError('startTime') ? 'error' : ''}
                      />
                      {errors.startTime && (
                        <span className="error-message">{errors.startTime}</span>
                      )}
                    </div>
                    
                    <div className={`form-group ${hasError('endTime') ? 'has-error' : ''}`}>
                      <label>End Time</label>
                      <input
                        type="time"
                        value={endTime}
                        onChange={(e) => {
                          setEndTime(e.target.value);
                          if (errors.endTime || errors.timeRange) {
                            setErrors({...errors, endTime: null, timeRange: null});
                          }
                        }}
                        className={hasError('endTime') ? 'error' : ''}
                      />
                      {errors.endTime && (
                        <span className="error-message">{errors.endTime}</span>
                      )}
                    </div>
                  </div>
                  
                  {/* Time Range Error */}
                  {errors.timeRange && (
                    <div className="error-message time-range-error">
                      {errors.timeRange}
                    </div>
                  )}
                  
                  {/* Form Buttons */}
                  <div className="form-buttons">
                    <button type="submit" className="btn-primary">
                      Create Booking
                    </button>
                    <button 
                      type="button" 
                      className="btn-secondary"
                      onClick={closeModal}
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default BookingForm;