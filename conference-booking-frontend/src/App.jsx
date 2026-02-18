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
//     ├── SearchFilter (sends FILTERS up via callbacks)
//     ├── BookingForm (sends NEW bookings up via onAdd)
//     └── BookingList (receives bookings down via props)
//
// Assignment Additions:
// • Async data fetching with useEffect
// • Loading + Error + Success UI states
// • Retry mechanism
// • Memory-safe async handling
//
// ============================================

import { useState, useEffect } from "react";
import { fetchAllBookings } from "./Services/bookingService";

import Navbar from './components/NavBar/Navbar';
import Footer from './components/Footer/Footer';
import BookingList from './components/BookingCard/BookingList';
import BookingForm from './components/BookingForm/BookingForm';
import SearchFilter from './components/SearchFilter/SearchFilter';
import './App.css';

function App() {

  // ==========================================
  // STATE (Component Memory)
  // ==========================================

  // Assignment rule: start with EMPTY data — no inline mock data
  const [bookings, setBookings] = useState([]);

  // Async UI state controls
  const [isLoading, setIsLoading] = useState(false); // shows loading UI
  const [error, setError] = useState(null);          // shows error UI

  // Retry trigger key — safe dependency to re-run fetch effect
  const [retryKey, setRetryKey] = useState(0);

  // Search & filter state (existing app logic)
  const [searchTerm, setSearchTerm] = useState('');
  const [filters, setFilters] = useState({
    room: 'all',
    status: 'all'
  });

  // ==========================================
  // ASYNC SIDE EFFECT — DATA FETCHING
  // ==========================================
  //
  // This effect synchronizes UI with external data.
  //
  // Runs:
  // • On first mount
  // • When retryKey changes (Retry button pressed)
  //
  // Safety features:
  // • cancellation guard prevents memory leaks
  // • avoids infinite loop by NOT depending on bookings
  //

  useEffect(() => {
    // Cancellation guard — prevents state update if unmounted
    let cancelled = false;

    async function loadBookings() {

      // Enter loading state → show loading UI
      setIsLoading(true);
      setError(null);

      try {
        const data = await fetchAllBookings();

        // Success — only update if still mounted
        if (!cancelled) {
          setBookings(data);
        }

      } catch (err) {

        // Failure — store error → show error UI
        if (!cancelled) {
          setError(err.message);
        }

      } finally {

        // Exit loading state
        if (!cancelled) {
          setIsLoading(false);
        }
      }
    }

    loadBookings();

    // Cleanup — prevents async race-condition updates
    return () => {
      cancelled = true;
    };

  }, [retryKey]); // Safe dependency trigger (NOT bookings!)



  // ==========================================
  // DERIVED STATE (Calculated — NOT stored)
  // ==========================================

  // Filter bookings based on search + filters
  const filteredBookings = bookings.filter(booking => {

    const matchesSearch =
      searchTerm === '' ||
      booking.roomName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      booking.customerName?.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesRoom =
      filters.room === 'all' ||
      booking.roomName === filters.room;

    const matchesStatus =
      filters.status === 'all' ||
      booking.status === filters.status;

    return matchesSearch && matchesRoom && matchesStatus;
  });

  // Dashboard counters (derived — no useEffect needed)
  const totalBookings = bookings.length;
  const pendingBookings = bookings.filter(b => b.status === 'pending').length;
  const confirmedBookings = bookings.filter(b => b.status === 'confirmed').length;



  // ==========================================
  // EVENT HANDLERS
  // ==========================================

  const handleAddBooking = (newBookingData) => {
    const newBooking = {
      id: Date.now(),
      ...newBookingData,
      status: 'pending'
    };

    // Local optimistic update
    setBookings([newBooking, ...bookings]);
  };

  const handleDeleteBooking = (bookingId) => {
    if (window.confirm('Cancel this booking?')) {
      setBookings(bookings.filter(b => b.id !== bookingId));
    }
  };

  const handleSearchChange = (term) => {
    setSearchTerm(term);
  };

  const handleFilterChange = (newFilters) => {
    setFilters({
      room: newFilters.room,
      status: newFilters.status
    });
  };



  // ==========================================
  // RENDER UI
  // ==========================================

  return (
    <div className="app-container">
      <Navbar />

      <main className="main-content">

        <SearchFilter
          onSearchChange={handleSearchChange}
          onFilterChange={handleFilterChange}
        />

        <BookingForm onAdd={handleAddBooking} />

        {/* ======================================
           RESILIENT UI STATES (Assignment Core)

           Loading → Error → Success rendering
           prevents blank screens & crashes
        ====================================== */}

        {/* Loading State */}
        {isLoading && (
          <div className="loading-message">
            Loading bookings from server...
          </div>
        )}

        {/* Error State + Retry */}
        {error && (
          <div className="error-message">
            <p>{error}</p>

            {/* Retry increments retryKey → re-triggers effect */}
            <button onClick={() => setRetryKey(k => k + 1)}>
              Retry
            </button>
          </div>
        )}

        {/* Success State — show data only when safe */}
        {!isLoading && !error && (
          <>
            <BookingList
              bookings={filteredBookings}
              onDeleteBooking={handleDeleteBooking}
            />

            {filteredBookings.length === 0 && bookings.length > 0 && (
              <div className="no-results-message">
                No bookings match your search criteria
              </div>
            )}
          </>
        )}

      </main>

      <Footer />
    </div>
  );
}

export default App;
