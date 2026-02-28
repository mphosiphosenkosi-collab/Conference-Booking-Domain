// src/services/bookingService.js

// Mock data for simulation
const MOCK_BOOKINGS = [
  {
    id: 1,
    conferenceName: 'Quarterly Business Review',
    room: 'A',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 86400000).toISOString().split('T')[0],
    attendees: 24
  },
  {
    id: 2,
    conferenceName: 'Client Strategy Meeting',
    room: 'B',
    status: 'pending',
    category: 'client',
    date: new Date(Date.now() + 172800000).toISOString().split('T')[0],
    attendees: 8
  },
  {
    id: 3,
    conferenceName: 'Product Launch Planning',
    room: 'C',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 259200000).toISOString().split('T')[0],
    attendees: 45
  },
  {
    id: 4,
    conferenceName: 'Board of Directors',
    room: 'D',
    status: 'confirmed',
    category: 'internal',
    date: new Date(Date.now() + 345600000).toISOString().split('T')[0],
    attendees: 15
  },
  {
    id: 5,
    conferenceName: 'Vendor Negotiations',
    room: 'A',
    status: 'cancelled',
    category: 'client',
    date: new Date(Date.now() - 86400000).toISOString().split('T')[0],
    attendees: 6
  }
];

/**
 * Simulates network delay between 500-2500ms
 */
const simulateDelay = () => {
  const delay = Math.floor(Math.random() * 2000) + 500; // 500-2500ms
  return new Promise(resolve => setTimeout(resolve, delay));
};

/**
 * Simulates flaky API (20% failure rate)
 */
const simulateFlakyApi = () => {
  const shouldFail = Math.random() < 0.2; // 20% chance of failure
  if (shouldFail) {
    throw new Error('Server Error: Unable to fetch bookings. Please try again.');
  }
};

/**
 * Fetches all bookings with simulated network conditions
 * @param {AbortSignal} signal - For cancellation
 * @returns {Promise<Array>}
 */
export const fetchAllBookings = async (signal) => {
  await simulateDelay();
  
  // Check if aborted
  if (signal?.aborted) {
    throw new DOMException('Aborted', 'AbortError');
  }

  simulateFlakyApi(); // 20% chance of failure

  // Return mock data with proper transformation
  return MOCK_BOOKINGS.map(booking => ({
    ...booking,
    // Ensure dates are fresh
    date: booking.id === 1 ? new Date(Date.now() + 86400000).toISOString().split('T')[0] :
          booking.id === 2 ? new Date(Date.now() + 172800000).toISOString().split('T')[0] :
          booking.id === 3 ? new Date(Date.now() + 259200000).toISOString().split('T')[0] :
          booking.id === 4 ? new Date(Date.now() + 345600000).toISOString().split('T')[0] :
          booking.date
  }));
};

/**
 * Creates a new booking with simulated network conditions
 * @param {Object} bookingData
 * @returns {Promise<Object>}
 */
export const createBooking = async (bookingData) => {
  const validation = validateBooking(bookingData);

  if (!validation.isValid) {
    throw new Error(validation.errors.join(', '));
  }

  await simulateDelay();
  simulateFlakyApi();

  // Create new booking with generated ID
  const newBooking = {
    id: Date.now(), // Temporary unique ID
    ...bookingData,
    status: 'pending' // New bookings start as pending
  };

  return newBooking;
};

/**
 * Deletes a booking by ID
 * @param {number} id
 * @returns {Promise<void>}
 */
export const deleteBooking = async (id) => {
  await simulateDelay();
  simulateFlakyApi();
  
  // Success - no return value needed
  return;
};

// Keep your existing validateBooking, filterBookings, and getFilterOptions functions
const validateBooking = (bookingData) => {
  const errors = [];

  if (!bookingData.conferenceName?.trim()) {
    errors.push('Conference name is required');
  }

  if (!bookingData.room) {
    errors.push('Room selection is required');
  }

  if (!bookingData.date) {
    errors.push('Date is required');
  }

  if (bookingData.attendees < 1 || bookingData.attendees > 100) {
    errors.push('Attendees must be between 1 and 100');
  }

  return {
    isValid: errors.length === 0,
    errors
  };
};

export const filterBookings = (bookings, filters) => {
  if (!bookings || !Array.isArray(bookings)) return [];

  return bookings.filter(booking => {
    const matchesSearch = !filters.searchTerm ||
      booking.conferenceName.toLowerCase().includes(filters.searchTerm.toLowerCase()) ||
      booking.room.toLowerCase().includes(filters.searchTerm.toLowerCase());

    const matchesRoom = !filters.room || booking.room === filters.room;
    const matchesStatus = !filters.status || booking.status === filters.status;
    const matchesCategory = !filters.category || booking.category === filters.category;

    return matchesSearch && matchesRoom && matchesStatus && matchesCategory;
  });
};

export const getFilterOptions = (bookings) => {
  if (!bookings || !Array.isArray(bookings)) {
    return { rooms: [], statuses: [], categories: [] };
  }

  return {
    rooms: [...new Set(bookings.map(b => b.room))].sort(),
    statuses: [...new Set(bookings.map(b => b.status))].sort(),
    categories: [...new Set(bookings.map(b => b.category))].sort()
  };
};