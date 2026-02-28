// src/components/Dashboard/Dashboard.jsx
import { useState, useEffect, useCallback } from "react";
import BookingForm from "../BookingForm/BookingForm"; // Keep this for the FAB button
import BookingList from "../BookingCard/BookingList";
import SearchFilter from "../SearchFilter/SearchFilter";
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
  
  // Retry mechanism
  const [retryKey, setRetryKey] = useState(0);

  // Fetch bookings
  const fetchBookings = useCallback(async () => {
    setLoading(true);
    setError(null);
    
    try {
      const data = await bookingService.fetchAllBookings();
      setBookings(data);
      setFilteredBookings(data);
    } catch (err) {
      setError(err.message);
      toast.error('Failed to load bookings');
    } finally {
      setLoading(false);
    }
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

      {/* Booking Form FAB - This renders the floating button */}
      <BookingForm onSubmit={handleCreateBooking} />

      {/* Bookings Grid - Full width now */}
      <div className="bookings-grid">
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
    </div>
  );
};

export default Dashboard;