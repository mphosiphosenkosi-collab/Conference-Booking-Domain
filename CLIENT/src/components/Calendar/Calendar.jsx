// src/components/Calendar/Calendar.jsx
import { useState, useEffect } from 'react';
import './Calendar.css';

const Calendar = ({ userRole = 'user', userName = 'Guest' }) => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [bookings, setBookings] = useState([]);
  const [viewMode, setViewMode] = useState('month');

  // Mock bookings data based on user role
  useEffect(() => {
    const mockBookings = generateMockBookings(userRole);
    setBookings(mockBookings);
  }, [userRole]);

  const generateMockBookings = (role) => {
    const baseBookings = [
      { id: 1, title: 'Team Meeting', date: '2026-03-15', time: '10:00', room: 'A', status: 'confirmed', attendees: 8 },
      { id: 2, title: 'Client Presentation', date: '2026-03-15', time: '14:00', room: 'B', status: 'confirmed', attendees: 12 },
      { id: 3, title: 'Board Meeting', date: '2026-03-16', time: '09:00', room: 'C', status: 'confirmed', attendees: 15 },
      { id: 4, title: 'Interview', date: '2026-03-16', time: '11:00', room: 'A', status: 'pending', attendees: 3 },
    ];

    switch(role) {
      case 'admin':
        return baseBookings;
      case 'receptionist':
        return baseBookings.filter(b => b.date >= new Date().toISOString().split('T')[0]);
      case 'user':
        return baseBookings.filter(b => b.category === 'internal');
      default:
        return baseBookings.slice(0, 2);
    }
  };

  const getDaysInMonth = (date) => {
    const year = date.getFullYear();
    const month = date.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    
    const days = [];
    const startDay = firstDay.getDay();
    
    for (let i = 0; i < startDay; i++) {
      days.push(null);
    }
    
    for (let i = 1; i <= lastDay.getDate(); i++) {
      days.push(new Date(year, month, i));
    }
    
    return days;
  };

  const getBookingsForDate = (date) => {
    if (!date) return [];
    const dateStr = date.toISOString().split('T')[0];
    return bookings.filter(b => b.date === dateStr);
  };

  const changeMonth = (increment) => {
    setCurrentDate(new Date(currentDate.getFullYear(), currentDate.getMonth() + increment, 1));
  };

  const formatDate = (date) => {
    return date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  };

  const days = getDaysInMonth(currentDate);
  const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

  const getRoleBadge = () => {
    const badges = {
      admin: { label: 'Administrator', color: 'linear-gradient(135deg, #f59e0b 0%, #f97316 100%)' },
      receptionist: { label: 'Receptionist', color: 'linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)' },
      user: { label: 'Team Member', color: 'linear-gradient(135deg, #10b981 0%, #059669 100%)' },
      guest: { label: 'Guest', color: 'linear-gradient(135deg, #6b7280 0%, #4b5563 100%)' }
    };
    return badges[userRole] || badges.guest;
  };

  const roleInfo = getRoleBadge();

  return (
    <div className="calendar-widget">
      <div className="calendar-header">
        <div className="calendar-title">
          <h3>📅 Calendar</h3>
          <span className="role-badge" style={{ background: roleInfo.color }}>
            {roleInfo.label}
          </span>
        </div>
        <div className="calendar-nav">
          <button onClick={() => changeMonth(-1)} className="nav-btn">←</button>
          <span className="current-month">{formatDate(currentDate)}</span>
          <button onClick={() => changeMonth(1)} className="nav-btn">→</button>
        </div>
      </div>

      <div className="calendar-weekdays">
        {weekDays.map(day => (
          <div key={day} className="weekday">{day}</div>
        ))}
      </div>

      <div className="calendar-grid">
        {days.map((date, index) => {
          const dayBookings = date ? getBookingsForDate(date) : [];
          const isToday = date && date.toDateString() === new Date().toDateString();
          const isSelected = date && date.toDateString() === selectedDate.toDateString();
          
          return (
            <div 
              key={index} 
              className={`calendar-day ${!date ? 'empty' : ''} ${isToday ? 'today' : ''} ${isSelected ? 'selected' : ''}`}
              onClick={() => date && setSelectedDate(date)}
            >
              {date && (
                <>
                  <span className="day-number">{date.getDate()}</span>
                  {dayBookings.length > 0 && (
                    <div className="booking-indicators">
                      {dayBookings.slice(0, 3).map((booking, i) => (
                        <div 
                          key={i} 
                          className={`booking-dot ${booking.status}`}
                          title={`${booking.title} - ${booking.time}`}
                        />
                      ))}
                    </div>
                  )}
                </>
              )}
            </div>
          );
        })}
      </div>

      <div className="upcoming-bookings">
        <h4>Upcoming</h4>
        <div className="upcoming-list">
          {bookings.slice(0, 3).map(booking => (
            <div key={booking.id} className="upcoming-item">
              <div className="upcoming-dot" style={{ background: roleInfo.color }} />
              <div className="upcoming-info">
                <span className="upcoming-title">{booking.title}</span>
                <span className="upcoming-meta">
                  {booking.date} • {booking.time}
                </span>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="user-welcome">
        <span className="welcome-icon">👋</span>
        <span className="welcome-text">Welcome, <strong>{userName}</strong>!</span>
      </div>
    </div>
  );
};

export default Calendar;