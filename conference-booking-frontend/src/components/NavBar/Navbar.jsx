// src/components/NavBar/Navbar.jsx
import { useState } from 'react';
import './Navbar.css';

function Navbar() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <nav className="navbar">
      <div className="nav-container">
        {/* Logo with gradient */}
        <div className="logo">
          <div className="logo-icon"></div>
          <span className="logo-text">Conference<span>Room Booking</span></span>
        </div>

        {/* Hamburger menu for mobile */}
        <button 
          className={`hamburger ${isMenuOpen ? 'active' : ''}`}
          onClick={() => setIsMenuOpen(!isMenuOpen)}
          aria-label="Toggle menu"
        >
          <span></span>
          <span></span>
          <span></span>
        </button>

        {/* Navigation Links */}
        <div className={`nav-links ${isMenuOpen ? 'active' : ''}`}>
          <a href="#" className="nav-link active">Home</a>
          <a href="#" className="nav-link">Rooms</a>
          <a href="#" className="nav-link">Bookings</a>
          <a href="#" className="nav-link">Calendar</a>
        </div>

        {/* Sign In Button - No icon */}
        <button className="signin-btn">
          Sign In
        </button>
      </div>
    </nav>
  );
}

export default Navbar;