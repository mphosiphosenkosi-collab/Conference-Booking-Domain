function Navbar() {
  return (
    <div style={{ 
      backgroundColor: '#2c3e50',  
      padding: '1rem 2rem', 
      color: 'white',
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'center',
      fontFamily: 'Arial, sans-serif',
      boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
    }}>
      {/* Left side - Logo/Brand */}
      <span style={{ 
        fontSize: '1.5rem',
        fontWeight: 'bold',
        display: 'flex',
        alignItems: 'center',
        gap: '10px'
      }}>
        <span style={{ fontSize: '2rem' }}></span>
        ConferenceBook
      </span>
      
      {/* Middle - Navigation Links */}
      <div style={{
        display: 'flex',
        gap: '2rem',
        fontSize: '1rem'
      }}>
        <span style={{ cursor: 'pointer' }}>Rooms</span>
        <span style={{ cursor: 'pointer' }}>Bookings</span>
        <span style={{ cursor: 'pointer' }}>Calendar</span>
      </div>
      
      {/* Right side - Assignment tag */}
      <span style={{ 
        backgroundColor: '#34495e',  // Slightly lighter than navbar
        padding: '0.5rem 1rem',
        borderRadius: '20px',         // Rounded corners
        fontSize: '0.9rem'
      }}>
        Assignment 7.1 - week 7 - Component Architecture & Static UI 
      </span>
    </div>
  );
}

export default Navbar;