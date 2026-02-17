// ============================================
// APP.JSX - The Root Component
// ============================================
//
// This is the BRAIN of our application.
// It owns the state and coordinates all other components.
//
// DATA FLOW:
//   App (owns state)
//     ├── Navbar (just display)
//     ├── BookingForm (sends NEW bookings UP via onAdd)
//     └── BookingList (receives bookings DOWN via props)
//
// ============================================

// STEP 1: Import what we need
// ----------------------------
// useState - gives our component memory (state)
import { useState } from 'react';

// Import all our UI components
import Navbar from './components/NavBar/Navbar';
import Footer from './components/Footer/Footer';
import BookingList from './components/BookingCard/BookingList';
import BookingForm from './components/BookingForm/BookingForm';

// Import our initial data (will be replaced by API later)
import mockBookings from './data/mockData';

// Import styles
import './App.css';

// ============================================
// STEP 2: Define the App component
// ============================================
function App() {
  
  // ==========================================
  // STEP 3: Create State (Component's Memory)
  // ==========================================
  // useState returns: [currentValue, functionToUpdateValue]
  //
  // const [bookings, setBookings] = useState(initialValue)
  //   - bookings:     The CURRENT array of bookings (read it)
  //   - setBookings:  The FUNCTION to update bookings (write with it)
  //   - mockBookings: Starting data so page isn't empty
  //
  const [bookings, setBookings] = useState(mockBookings);
  
  // ==========================================
  // STEP 4: Derived State (Calculated Values)
  // ==========================================
  // These are NOT new state variables - they're just CALCULATED
  // from existing state. When bookings changes, these update automatically!
  //
  const totalBookings = bookings.length;
  const pendingBookings = bookings.filter(b => b.status === 'pending').length;
  const confirmedBookings = bookings.filter(b => b.status === 'confirmed').length;
  
  // ==========================================
  // STEP 5: Event Handler (Lifting State Up)
  // ==========================================
  // This function will be PASSED DOWN to BookingForm.
  // The form will CALL it when user submits new booking.
  //
  // Why defined here? Because ONLY the component that owns state
  // (App) should update it. The form just says "hey, here's new data!"
  //
  const handleAddBooking = (newBookingData) => {
    console.log("📝 App received new booking:", newBookingData);
    
    // Create a complete booking object with unique ID
    // Date.now() gives milliseconds since 1970 - always unique!
    const newBooking = {
      id: Date.now(),           // ✅ Unique ID guaranteed
      ...newBookingData,        // Spread all form data (roomName, userName, etc.)
      status: 'pending'         // New bookings start as pending
    };
    
    // ========================================
    // IMMUTABLE UPDATE - CRITICAL!
    // ========================================
    // ❌ WRONG: bookings.push(newBooking) 
    //    - Mutates existing array
    //    - React won't detect change (same array reference)
    //    - UI won't update!
    //
    // ✅ CORRECT: Create NEW array with spread operator
    //    - [...bookings] makes a copy of all existing bookings
    //    - We add newBooking at the beginning
    //    - React sees NEW array → detects change → re-renders!
    //
    setBookings([newBooking, ...bookings]);
    
    // The form will clear itself - we don't do that here
  };
  
  // ==========================================
  // STEP 6: Render the UI
  // ==========================================
  return (
    <div className="app-container">
      {/* Navbar - no props needed, just displays */}
      <Navbar />
      
      {/* Main content area */}
      <main className="main-content">
        
        {/* ======================================
            Section 1: Dashboard Stats
            Shows real-time counts from our state
            ====================================== */}
        <div className="stats-container">
          <div className="stat-card">
            <span className="stat-label">Total Bookings</span>
            <span className="stat-value">{totalBookings}</span>
          </div>
          <div className="stat-card">
            <span className="stat-label">Pending</span>
            <span className="stat-value">{pendingBookings}</span>
          </div>
          <div className="stat-card">
            <span className="stat-label">Confirmed</span>
            <span className="stat-value">{confirmedBookings}</span>
          </div>
        </div>
        
        {/* ======================================
            Section 2: Booking Form
            Pass DOWN the function so it can send UP new bookings
            ====================================== */}
        <BookingForm onAdd={handleAddBooking} />
        
        {/* ======================================
            Section 3: Booking List
            Pass DOWN the bookings data to display
            ====================================== */}
        <BookingList bookings={bookings} />
        
      </main>
      
      {/* Footer - no props needed */}
      <Footer />
    </div>
  );
}

// ============================================
// STEP 7: Export so other files can import it
// ============================================
export default App;