// src/components/layout/Navbar.jsx
import './Navbar.css';  // Import the CSS file


function Navbar() {
  return (
    <div className="navbar">  
      
      {/* Left side - Logo/Brand */}
      <span className="navbar-logo">
        <span className="navbar-logo-icon"></span>
        Conference Room Booking
      </span>
      
      {/* Middle - Navigation Links */}
      <div className="navbar-links">
        <span className="navbar-link">Rooms</span>
        <span className="navbar-link">Bookings</span>
        <span className="navbar-link">Calendar</span>
      </div>
      
      {/* Right side - Registre tag */}
      <span className="navbar-badge">
        Sign In
      </span>
    </div>
  );
}

export default Navbar;