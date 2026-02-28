// src/components/Dashboard/Dashboard.jsx
import { useState, useEffect, useCallback } from "react";
import BookingForm from "../BookingForm/BookingForm";
import BookingList from "../BookingCard/BookingList";
import SearchFilter from "../SearchFilter/SearchFilter";
import Calendar from "../Calendar/Calendar";
import * as bookingService from "../../services/bookingService";
import { toast } from "react-toastify";
import "./Dashboard.css";

const Dashboard = () => {
  // State management
  const [bookings, setBookings] = useState([]);
  const [filteredBookings, setFilteredBookings] = useState([]);
  const [filters, setFilters] = useState({
    searchTerm: '',
    room: '',
    status: '',
    category: ''
  });

  // UI State
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // User state (in real app, this would come from auth context)
  const [currentUser, setCurrentUser] = useState({
    role: 'admin', // Can be 'admin', 'receptionist', 'user', 'guest'
    name: 'Siphosenkosi'
  });

  // Retry mechanism
  const [retryKey, setRetryKey] = useState(0);

  // Fetch bookings
  // In Dashboard.jsx, update the fetchBookings function:

  const fetchBookings = useCallback(async () => {
    const abortController = new AbortController();

    setLoading(true);
    setError(null);

    try {
      const data = await bookingService.fetchAllBookings(abortController.signal);
      setBookings(data);
      setFilteredBookings(data);
      toast.success('Bookings loaded successfully');
    } catch (err) {
      if (err.name === 'AbortError') {
        console.log('Fetch aborted');
        return;
      }
      setError(err.message);
      toast.error('Failed to load bookings');
    } finally {
      setLoading(false);
    }

    return () => abortController.abort();
  }, []);

  // Initial fetch and retry
  useEffect(() => {
    fetchBookings();
  }, [fetchBookings, retryKey]);

  // Apply filters whenever bookings or filters change
  useEffect(() => {
    const filtered = bookingService.filterBookings(bookings, filters);
    setFilteredBookings(filtered);
  }, [bookings, filters]);

  // Handlers
  const handleFilterChange = (newFilters) => {
    setFilters(prev => ({ ...prev, ...newFilters }));
  };

  const handleCreateBooking = async (bookingData) => {
    try {
      const newBooking = await bookingService.createBooking(bookingData);
      setBookings(prev => [...prev, newBooking]);
      toast.success('Booking created successfully');
    } catch (err) {
      toast.error(err.message);
    }
  };

  const handleDeleteBooking = async (id) => {
    try {
      await bookingService.deleteBooking(id);
      setBookings(prev => prev.filter(booking => booking.id !== id));
      toast.success('Booking deleted successfully');
    } catch (err) {
      toast.error(err.message);
    }
  };

  const handleRetry = () => {
    setRetryKey(prev => prev + 1);
  };

  // Get filter options from service
  const filterOptions = bookingService.getFilterOptions(bookings);

  {
    !loading && !error && (
      <div className="total-bookings-counter">
        <div className="counter-badge">
          <span className="counter-label">Total Bookings</span>
          <span className="counter-value">{filteredBookings.length}</span>
        </div>
        {filters && Object.values(filters).some(Boolean) && (
          <span className="filtered-hint">
            (filtered from {bookings.length})
          </span>
        )}
      </div>
    )
  }



  return (
    <div className="dashboard">
      {/* Filter Section */}
      <div className="filter-section">
        <SearchFilter
          filters={filters}
          onFilterChange={handleFilterChange}
          options={filterOptions}
        />
      </div>

      {/* Booking Form FAB */}
      <BookingForm onSubmit={handleCreateBooking} />

      {/* Main Content Grid - Now with Calendar */}
      <div className="dashboard-grid">
        {/* Left Column - Bookings Grid */}
        <div className="bookings-column">
          {error && (
            <div className="error-container">
              <p className="error-message">{error}</p>
              <button onClick={handleRetry} className="retry-button">
                Retry
              </button>
            </div>
          )}

          {loading ? (
            <div className="loading-state">
              <div className="loading-spinner"></div>
              <p>Loading bookings...</p>
            </div>
          ) : (
            <BookingList
              bookings={filteredBookings}
              onDelete={handleDeleteBooking}
            />
          )}
        </div>

        {/* Right Column - Calendar */}
        <div className="calendar-column">
          <Calendar
            userRole={currentUser.role}
            userName={currentUser.name}
          />
        </div>
      </div>
    </div>
  );
};

export default Dashboard;