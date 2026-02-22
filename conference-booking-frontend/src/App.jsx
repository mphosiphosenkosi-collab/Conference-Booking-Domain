// ============================================
// APP.JSX - ROOT LAYOUT CONTROLLER
// ============================================

import './App.css';
import { useState, useEffect } from "react";
import { fetchAllBookings } from "./Services/bookingService";

import Heartbeat from "./components/Heartbeat/Heartbeat";
import Navbar from './components/NavBar/Navbar';
import Footer from './components/Footer/Footer';
import BookingList from './components/BookingCard/BookingList';
import BookingForm from './components/BookingForm/BookingForm';
import SearchFilter from './components/SearchFilter/SearchFilter';

function App() {

  // ============================
  // STATE
  // ============================

  const [bookings, setBookings] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [retryKey, setRetryKey] = useState(0);

  const [toast, setToast] = useState(null);

  const [category, setCategory] = useState("all");
  const [searchTerm, setSearchTerm] = useState('');
  const [filters, setFilters] = useState({
    room: 'all',
    status: 'all'
  });

  function showToast(message) {
    setToast(message);
    setTimeout(() => setToast(null), 3000);
  }

  // ============================
  // DATA FETCHING
  // ============================

  useEffect(() => {
    const controller = new AbortController();

    async function loadBookings() {
      setIsLoading(true);
      setError(null);

      try {
        const data = await fetchAllBookings(controller.signal);
        setBookings(data);
        showToast("Data synced successfully");
      } catch (err) {
        if (err.message === "Request aborted") return;
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    }

    loadBookings();

    return () => controller.abort();

  }, [retryKey]);

  // ============================
  // HEARTBEAT DEMO
  // ============================

  useEffect(() => {
    const intervalId = setInterval(() => {
      console.log("App heartbeat — running");
    }, 10000);

    return () => clearInterval(intervalId);
  }, []);

  // ============================
  // DERIVED STATE
  // ============================

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

    const matchesCategory =
      category === "all" ||
      booking.category === category;

    return matchesSearch && matchesRoom && matchesStatus && matchesCategory;
  });

  // ============================
  // EVENT HANDLERS
  // ============================

  const handleAddBooking = (newBookingData) => {
    const newBooking = {
      id: Date.now(),
      ...newBookingData,
      status: 'pending'
    };

    setBookings(prev => [newBooking, ...prev]);
  };

  const handleDeleteBooking = (bookingId) => {
    if (window.confirm('Cancel this booking?')) {
      setBookings(prev => prev.filter(b => b.id !== bookingId));
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

  // ============================
  // RENDER
  // ============================

  return (
    <div className="app-layout">

      {/* SIDEBAR */}
      <Navbar />

      {/* RIGHT SIDE CONTENT */}
      <div className="app-content">

        <main className="main-content">

          <Heartbeat />

          <SearchFilter
            onSearchChange={handleSearchChange}
            onFilterChange={handleFilterChange}
          />

          <div className="category-filter">
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
            >
              <option value="all">All</option>
              <option value="internal">Internal</option>
              <option value="client">Client</option>
            </select>
          </div>

          <BookingForm onAdd={handleAddBooking} />

          {/* LOADING STATE */}
          {isLoading && bookings.length === 0 && (
            <div className="loading-message">
              Loading bookings...
            </div>
          )}

          {/* REFRESH STATE */}
          {isLoading && bookings.length > 0 && (
            <div className="refreshing-message">
              Refreshing data...
            </div>
          )}

          {/* ERROR STATE */}
          {error && (
            <div className="error-message">
              <p>{error}</p>
              <button onClick={() => setRetryKey(k => k + 1)}>
                Retry
              </button>
            </div>
          )}

          {/* SUCCESS STATE */}
          {!isLoading && !error && (
            <>
              <BookingList
                bookings={filteredBookings}
                onDeleteBooking={handleDeleteBooking}
              />

              {filteredBookings.length === 0 && bookings.length > 0 && (
                <div className="no-results-message">
                  No bookings match your filters.
                </div>
              )}
            </>
          )}

        </main>

        {/* FOOTER LOCKED TO BOTTOM */}
        <Footer />

      </div>

      {/* TOAST */}
      {toast && (
        <div className="toast">
          {toast}
        </div>
      )}

    </div>
  );
}

export default App;