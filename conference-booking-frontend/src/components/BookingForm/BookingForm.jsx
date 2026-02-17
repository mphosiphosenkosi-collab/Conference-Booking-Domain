// src/components/BookingForm/BookingForm.jsx
import { useState } from 'react';
import './BookingForm.css';

function BookingForm({ onAdd }) {
  const [roomName, setRoomName] = useState('');
  const [userName, setUserName] = useState('');
  const [date, setDate] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  
  // NEW: State for validation errors
  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors = {};
    
    if (!roomName.trim()) newErrors.roomName = 'Room name is required';
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
    
    // Clear form
    setRoomName('');
    setUserName('');
    setDate('');
    setStartTime('');
    setEndTime('');
  };

  // Helper function to check if a field has an error
  const hasError = (fieldName) => !!errors[fieldName];

  return (
    <div className="booking-form-wrapper">
      <div className="booking-form-container">
        <div className="booking-form-header">
          <span className="header-icon">📅</span>
          <h2>New Booking</h2>
        </div>
        <div className="form-subtitle">Fill in the details to create a booking</div>
        
        <form onSubmit={handleSubmit}>
          <div className="form-content">
            {/* Room Name Field */}
            <div className={`form-group ${hasError('roomName') ? 'has-error' : ''}`}>
              <label>Room Name</label>
              <input
                type="text"
                value={roomName}
                onChange={(e) => {
                  setRoomName(e.target.value);
                  // Clear error when user starts typing
                  if (errors.roomName) {
                    setErrors({...errors, roomName: null});
                  }
                }}
                placeholder="e.g., Conference Room A"
                className={hasError('roomName') ? 'error' : ''}
              />
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
              <label>Date</label>
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
              />
              {errors.date && (
                <span className="error-message">{errors.date}</span>
              )}
            </div>
            
            {/* Time Fields */}
            <div className="time-row">
              <div className={`form-group ${hasError('startTime') ? 'has-error' : ''}`}>
                <label>Start</label>
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
                <label>End</label>
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
            
            {/* Time Range Error (shows below both time fields) */}
            {errors.timeRange && (
              <div className="error-message time-range-error">
                ⚠️ {errors.timeRange}
              </div>
            )}
            
            {/* Form Buttons */}
            <div className="form-buttons">
              <button type="submit" className="btn-primary">
                <span>✨</span> Create Booking
              </button>
              <button 
                type="button" 
                className="btn-secondary"
                onClick={() => {
                  setRoomName('');
                  setUserName('');
                  setDate('');
                  setStartTime('');
                  setEndTime('');
                  setErrors({}); // Clear errors too!
                }}
              >
                <span>↺</span> Clear
              </button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}

export default BookingForm;