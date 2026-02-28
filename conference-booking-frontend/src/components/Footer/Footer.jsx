// src/components/Footer/Footer.jsx
import './Footer.css';

const Footer = () => {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="footer">
      <div className="footer-content">
        <div className="footer-section">
          <p>&copy; {currentYear} ConferenceHub. All rights reserved.</p>
        </div>
        
        <div className="footer-section">
          <span className="version">v2.0.0</span>
        </div>
        
        <div className="footer-links">
          <a href="#" onClick={(e) => e.preventDefault()}>Privacy</a>
          <span className="separator">•</span>
          <a href="#" onClick={(e) => e.preventDefault()}>Terms</a>
          <span className="separator">•</span>
          <a href="#" onClick={(e) => e.preventDefault()}>Support</a>
        </div>
      </div>
    </footer>
  );
};

export default Footer;