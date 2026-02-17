// src/components/layout/Footer.jsx
import './Footer.css';  // Import the CSS file

function Footer() {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="footer">
      <div className="footer-content">
        <p>© {currentYear} Conference Booking System</p>
        <p>
          <span role="img" aria-label="react">⚛️</span> 
          React Assignment 1.1
        </p>
      </div>
    </footer>
  );
}

export default Footer;