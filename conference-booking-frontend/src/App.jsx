import Navbar from './components/layout/Navbar';
import Footer from './components/layout/Footer';
import BookingCard from './components/features/BookingCard';
import mockBookings from './data/mockData';
import './App.css'

function App() {
  // Simple handler functions (temporary for now)
  const handleEdit = (bookingId) => {
    alert(`Edit booking #${bookingId}`);
  };
  
  const handleCancel = (bookingId) => {
    alert(`Cancel booking #${bookingId}`);
  };
  
  return (
    <div className="app-container">
      <Navbar />
      
      <main className="main-content">
        <h1>Current Bookings</h1>
        
        <div className="bookings-grid">
          {mockBookings.map(booking => (
            <BookingCard
              key={booking.id}
              id={booking.id}
              roomName={booking.roomName}
              date={booking.date}
              startTime={booking.startTime}
              endTime={booking.endTime}
              userName={booking.userName}
              status={booking.status}
              onEdit={handleEdit}
              onCancel={handleCancel}
            />
          ))}
        </div>
      </main>
      
      <Footer />
    </div>
  )
}

export default App;