// src/components/layout/Footer.jsx
function Footer() {
  const currentYear = new Date().getFullYear();
  
  return (
    <footer className="footer">
      <div className="footer-content">
        <p>© {currentYear} Conference Booking System</p>
        <p>React ⚛️ Assignment 1</p>
      </div>
    </footer>
  );
}

export default Footer;