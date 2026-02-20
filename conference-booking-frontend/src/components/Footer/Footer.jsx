// src/components/Footer/Footer.jsx
import './Footer.css';

function Footer() {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="footer">
      <div className="footer-content">
        <div className="footer-left">
          <p className="copyright">© {currentYear} Conference Room Booking System</p>
        </div>
        
        <div className="footer-right">
          <p className="assignment-info">
            React Assignment -
          </p>
        </div>
      </div>
      
      {/* Decorative gradient line */}
      <div className="footer-gradient"></div>
    </footer>
  );
}

export default Footer;