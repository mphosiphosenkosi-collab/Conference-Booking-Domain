// src/components/layout/Navbar.jsx
import './Navbar.css';  // Import the CSS file


function Navbar() {
  return (
    <div className="navbar">  {/* ← Now using className instead of style */}
      
      {/* Left side - Logo/Brand */}
      <span className="navbar-logo">
        <span className="navbar-logo-icon">🏨</span>
        ConferenceBook
      </span>
      
      {/* Middle - Navigation Links */}
      <div className="navbar-links">
        <span className="navbar-link">Rooms</span>
        <span className="navbar-link">Bookings</span>
        <span className="navbar-link">Calendar</span>
      </div>
      
      {/* Right side - Assignment tag */}
      <span className="navbar-badge">
        Assignment 7.1 - week 7 - Component Architecture & Static UI 
      </span>
    </div>
  );
}

export default Navbar;