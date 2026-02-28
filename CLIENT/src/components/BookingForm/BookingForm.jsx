// src/components/BookingForm/BookingForm.jsx
import { useState } from 'react';
import './BookingForm.css';

const BookingForm = ({ onSubmit }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [formData, setFormData] = useState({
    conferenceName: '',
    room: '',
    date: '',
    attendees: 10,
    category: 'internal'
  });
  
  // State for validation errors
  const [errors, setErrors] = useState({});

  // Available rooms for dropdown (matching your room codes)
  const availableRooms = [
    { value: 'A', label: 'Room A - Executive Suite' },
    { value: 'B', label: 'Room B - Conference Hall' },
    { value: 'C', label: 'Room C - Meeting Room' },
    { value: 'D', label: 'Room D - Board Room' }
  ];

  const validateForm = () => {
    const newErrors = {};
    
    if (!formData.conferenceName?.trim()) {
      newErrors.conferenceName = 'Conference name is required';
    }
    
    if (!formData.room) {
      newErrors.room = 'Please select a room';
    }
    
    if (!formData.date) {
      newErrors.date = 'Date is required';
    }
    
    if (formData.attendees < 1 || formData.attendees > 100) {
      newErrors.attendees = 'Attendees must be between 1 and 100';
    }
    
    if (!formData.category) {
      newErrors.category = 'Please select a category';
    }
    
    return newErrors;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    // Validate form
    const validationErrors = validateForm();
    
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    
    // Clear any previous errors
    setErrors({});
    
    // Submit booking
    onSubmit(formData);
    
    // Reset form and close modal
    setFormData({
      conferenceName: '',
      room: '',
      date: '',
      attendees: 10,
      category: 'internal'
    });
    setIsOpen(false);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'attendees' ? parseInt(value) || 0 : value
    }));
    
    // Clear error for this field when user starts typing
    if (errors[name]) {
      setErrors({...errors, [name]: null});
    }
  };

  // Helper function to check if a field has an error
  const hasError = (fieldName) => !!errors[fieldName];

  // Format date for display
  const formatDate = (dateString) => {
    if (!dateString) return 'Not set';
    const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
    return new Date(dateString).toLocaleDateString(undefined, options);
  };

  // Get room label by value
  const getRoomLabel = (roomValue) => {
    const room = availableRooms.find(r => r.value === roomValue);
    return room ? room.label : 'Not selected';
  };

  const openModal = () => {
    setIsOpen(true);
    document.body.style.overflow = 'hidden'; // Prevent background scrolling
  };

  const closeModal = () => {
    setIsOpen(false);
    document.body.style.overflow = 'unset'; // Restore scrolling
    // Reset form and errors when closing
    setFormData({
      conferenceName: '',
      room: '',
      date: '',
      attendees: 10,
      category: 'internal'
    });
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
                  <span className="overview-label">Conference</span>
                  <span className="overview-value">{formData.conferenceName || '—'}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Room</span>
                  <span className="overview-value">{getRoomLabel(formData.room)}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Date</span>
                  <span className="overview-value">{formatDate(formData.date)}</span>
                </div>
                <div className="overview-item">
                  <span className="overview-label">Attendees</span>
                  <span className="overview-value">{formData.attendees || '—'}</span>
                </div>
              </div>
            </div>

            {/* Booking Form */}
            <div className="booking-form-container">
              <div className="booking-form-header">
                <h2>Create New Booking</h2>
                <p className="form-subtitle">Fill in the details to reserve a conference room</p>
              </div>
              
              <form onSubmit={handleSubmit}>
                <div className="form-content">
                  {/* Conference Name */}
                  <div className={`form-group ${hasError('conferenceName') ? 'has-error' : ''}`}>
                    <label>
                      <span>📋</span> Conference Name
                    </label>
                    <input
                      type="text"
                      name="conferenceName"
                      value={formData.conferenceName}
                      onChange={handleChange}
                      placeholder="e.g., Quarterly Business Review"
                      className={hasError('conferenceName') ? 'error' : ''}
                    />
                    {errors.conferenceName && (
                      <span className="error-message">{errors.conferenceName}</span>
                    )}
                  </div>
                  
                  {/* Room Selection */}
                  <div className={`form-group ${hasError('room') ? 'has-error' : ''}`}>
                    <label>
                      <span>📍</span> Select Meeting Room
                    </label>
                    <select
                      name="room"
                      value={formData.room}
                      onChange={handleChange}
                      className={hasError('room') ? 'error' : ''}
                    >
                      <option value="">— Choose a room —</option>
                      {availableRooms.map(room => (
                        <option key={room.value} value={room.value}>
                          {room.label}
                        </option>
                      ))}
                    </select>
                    {errors.room && (
                      <span className="error-message">{errors.room}</span>
                    )}
                  </div>
                  
                  {/* Date Field */}
                  <div className={`form-group ${hasError('date') ? 'has-error' : ''}`}>
                    <label>
                      <span>📅</span> Select Date
                    </label>
                    <input
                      type="date"
                      name="date"
                      value={formData.date}
                      onChange={handleChange}
                      className={hasError('date') ? 'error' : ''}
                      min={new Date().toISOString().split('T')[0]}
                    />
                    {errors.date && (
                      <span className="error-message">{errors.date}</span>
                    )}
                  </div>
                  
                  {/* Attendees */}
                  <div className={`form-group ${hasError('attendees') ? 'has-error' : ''}`}>
                    <label>
                      <span>👥</span> Number of Attendees
                    </label>
                    <input
                      type="number"
                      name="attendees"
                      value={formData.attendees}
                      onChange={handleChange}
                      min="1"
                      max="100"
                      className={hasError('attendees') ? 'error' : ''}
                    />
                    {errors.attendees && (
                      <span className="error-message">{errors.attendees}</span>
                    )}
                  </div>
                  
                  {/* Category Selection */}
                  <div className={`form-group ${hasError('category') ? 'has-error' : ''}`}>
                    <label>
                      <span>🏷️</span> Booking Category
                    </label>
                    <select
                      name="category"
                      value={formData.category}
                      onChange={handleChange}
                      className={hasError('category') ? 'error' : ''}
                    >
                      <option value="internal">🏢 Internal Meeting</option>
                      <option value="client">🤝 Client Meeting</option>
                    </select>
                    {errors.category && (
                      <span className="error-message">{errors.category}</span>
                    )}
                  </div>
                  
                  {/* Form Buttons */}
                  <div className="form-buttons">
                    <button type="submit" className="btn-primary">
                      ✨ Create Booking
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
};

export default BookingForm;