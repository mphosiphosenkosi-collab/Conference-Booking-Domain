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

  // ==========================================
  // TOAST STATE (Extra Credit)
  // ==========================================
  // Controls small success popup message

  const [toast, setToast] = useState(null);

  function showToast(message) {
    setToast(message);
    setTimeout(() => setToast(null), 3000);
  }


  // Category filter state (Assignment Requirement)
  const [category, setCategory] = useState("all");


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

  // ==========================================
  // ASYNC FETCH EFFECT — EXTRA CREDIT VERSION
  // ==========================================
  //
  // Purpose:
  // • Fetch bookings from API
  // • Support request cancellation (AbortController)
  // • Prevent memory leaks
  // • Avoid infinite loops via safe dependencies
  // • Support retry button via retryKey trigger
  //
  // Extra Credit Features:
  // • AbortController cancels request if component unmounts
  // • Safe error handling (ignore abort errors)
  // • Compatible with stale-while-refresh UI pattern
  //

  useEffect(() => {

    // Create AbortController for this specific request
    // This lets us cancel the request if component unmounts
    const controller = new AbortController();

    async function loadBookings() {

      // Turn loading ON
      // If we already have data, UI will show "Refreshing..."
      setIsLoading(true);

      // Clear previous error
      setError(null);

      try {

        // Pass abort signal into service layer
        const data = await fetchAllBookings(controller.signal);

        // Save new data into state
        setBookings(data);

        //  Extra credit — success feedback hook
        // (only works if you added showToast helper)
        if (typeof showToast === "function") {
          showToast("Data sync successful");
        }

      } catch (err) {

        // If request was aborted — DO NOT show error UI
        if (err.message === "Request aborted") {
          console.log("Fetch cancelled safely");
          return;
        }

        // Real server error → show error UI
        setError(err.message);

      } finally {

        // Turn loading OFF
        setIsLoading(false);
      }
    }

    // Run the async loader
    loadBookings();

    // ======================================
    // CLEANUP FUNCTION
    // ======================================
    //
    // Runs when:
    // • Component unmounts
    // • Effect re-runs (retryKey changes)
    //
    // Cancels in-flight request to prevent:
    // • memory leaks
    // • setState on unmounted component
    //

    return () => {
      controller.abort();
    };


    // Dependency discipline:
    // Only retryKey triggers refetch
    // NOT bookings → prevents infinite loop

  }, [retryKey]);

  // ==========================================
  // HEARTBEAT EFFECT (Lifecycle Demonstration)
  // ==========================================

  // This effect demonstrates how to run a repeating timer while the component is alive.
  // Cleanup stops the timer when component unmounts.

  useEffect(() => {

    const intervalId = setInterval(() => {
      console.log("App heartbeat — still running");
    }, 10000); // every 10 seconds

    // CLEANUP FUNCTION
    // This runs when component unmounts
    return () => {
      clearInterval(intervalId);
      console.log("Heartbeat stopped — component unmounted");
    };

  }, []); // empty dependency → run once




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

    //  Assignment 1.3 Category Filter
    const matchesCategory =
      category === "all" ||
      booking.category === category;

    return matchesSearch && matchesRoom && matchesStatus && matchesCategory;
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


        {/* Assignment Category Filter */}
        <select
          value={category}
          onChange={(e) => setCategory(e.target.value)}
        >
          <option value="all">All</option>
          <option value="internal">Internal</option>
          <option value="client">Client</option>
        </select>

        {/* Add Booking Form (sends new bookings up via onAdd callback) */}
        <BookingForm onAdd={handleAddBooking} />


        {/* ======================================
           RESILIENT UI STATES (Assignment Core)

           Loading → Error → Success rendering
           prevents blank screens & crashes
        ====================================== */}

        {/* First load — no data yet */}
        {isLoading && bookings.length === 0 && (
          <div className="loading-message">
            Loading bookings from server...
          </div>
        )}

        {/* Background refresh — keep showing old data */}
        {isLoading && bookings.length > 0 && (
          <div className="refreshing-message">
            Refreshing data...
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

      {/* Extra Credit Toast Notification */}
      {toast && (
        <div className="toast">
          {toast}
        </div>
      )}


      <Footer />
    </div>
  );
}

export default App;
