// src/components/BookingForm/BookingForm.jsx
// STEP 6: Use the function from App

import { useState } from 'react';

// ✅ STEP 6: Accept the function as a prop
function BookingForm({ onAddBooking }) {
  const [roomName, setRoomName] = useState('');
  const [userName, setUserName] = useState('');
  const [date, setDate] = useState('');
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  
  const handleSubmit = (e) => {
    e.preventDefault();
    
    // ✅ STEP 6: Call the function from App instead of alert
    onAddBooking({
      roomName,
      userName,
      date,
      startTime,
      endTime
    });
    
    // Clear the form
    setRoomName('');
    setUserName('');
    setDate('');
    setStartTime('');
    setEndTime('');
  };
  
  console.log("📝 Form state:", { roomName, userName, date, startTime, endTime });
  
  // Styles (same as before)
  const formStyle = {
    backgroundColor: '#f8f9fa',
    padding: '20px',
    borderRadius: '8px',
    marginBottom: '20px'
  };
  
  const inputStyle = {
    width: '100%',
    padding: '8px',
    marginBottom: '10px',
    border: '1px solid #ddd',
    borderRadius: '4px'
  };
  
  const buttonStyle = {
    backgroundColor: '#3498db',
    color: 'white',
    padding: '10px 20px',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer'
  };
  
  return (
    <div style={formStyle}>
      <h2>➕ Add New Booking</h2>
      
      <form onSubmit={handleSubmit}>
        <div>
          <label>Room Name:</label>
          <input
            type="text"
            style={inputStyle}
            placeholder="e.g., Conference Room A"
            value={roomName}
            onChange={(e) => setRoomName(e.target.value)}
          />
        </div>
        
        <div>
          <label>Your Name:</label>
          <input
            type="text"
            style={inputStyle}
            placeholder="e.g., John Smith"
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
          />
        </div>
        
        <div>
          <label>Date:</label>
          <input
            type="date"
            style={inputStyle}
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />
        </div>
        
        <div>
          <label>Start Time:</label>
          <input
            type="time"
            style={inputStyle}
            value={startTime}
            onChange={(e) => setStartTime(e.target.value)}
          />
        </div>
        
        <div>
          <label>End Time:</label>
          <input
            type="time"
            style={inputStyle}
            value={endTime}
            onChange={(e) => setEndTime(e.target.value)}
          />
        </div>
        
        <button type="submit" style={buttonStyle}>
          Add Booking
        </button>
      </form>
    </div>
  );
}

export default BookingForm;