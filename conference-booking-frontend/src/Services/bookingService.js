// src/services/bookingService.js
const API_BASE_URL = 'https://jsonplaceholder.typicode.com';

/**
 * Fetches all bookings from the API
 * @returns {Promise<Array>}
 */
export const fetchAllBookings = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/posts`);
    if (!response.ok) throw new Error('Failed to fetch bookings');
    const data = await response.json();
    return transformApiResponseToBookings(data);
  } catch (error) {
    throw new Error(`Fetch error: ${error.message}`);
  }
};

/**
 * Transforms API response to domain booking objects
 * @param {Array} apiData 
 * @returns {Array}
 */
const transformApiResponseToBookings = (apiData) => {
  return apiData.slice(0, 10).map((item, index) => ({
    id: item.id,
    conferenceName: `Conference ${item.title.substring(0, 20)}`,
    room: ['A', 'B', 'C', 'D'][Math.floor(Math.random() * 4)],
    status: ['confirmed', 'pending', 'cancelled'][Math.floor(Math.random() * 3)],
    category: ['internal', 'client'][Math.floor(Math.random() * 2)],
    date: new Date(Date.now() + index * 86400000).toISOString().split('T')[0],
    attendees: Math.floor(Math.random() * 50) + 10
  }));
};

/**
 * Filters bookings based on multiple criteria
 * @param {Array} bookings 
 * @param {Object} filters 
 * @returns {Array}
 */
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

/**
 * Validates booking data before creation
 * @param {Object} bookingData 
 * @returns {Object} { isValid: boolean, errors: Array }
 */
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

/**
 * Creates a new booking
 * @param {Object} bookingData 
 * @returns {Promise<Object>}
 */
export const createBooking = async (bookingData) => {
  const validation = validateBooking(bookingData);
  
  if (!validation.isValid) {
    throw new Error(validation.errors.join(', '));
  }
  
  try {
    const response = await fetch(`${API_BASE_URL}/posts`, {
      method: 'POST',
      body: JSON.stringify({
        title: bookingData.conferenceName,
        body: JSON.stringify({
          room: bookingData.room,
          date: bookingData.date,
          attendees: bookingData.attendees,
          category: bookingData.category
        }),
        userId: 1
      }),
      headers: {
        'Content-type': 'application/json; charset=UTF-8',
      },
    });
    
    if (!response.ok) throw new Error('Failed to create booking');
    const data = await response.json();
    
    return {
      id: data.id,
      conferenceName: bookingData.conferenceName,
      room: bookingData.room,
      status: 'pending',
      category: bookingData.category,
      date: bookingData.date,
      attendees: bookingData.attendees
    };
  } catch (error) {
    throw new Error(`Create error: ${error.message}`);
  }
};

/**
 * Deletes a booking by ID
 * @param {number} id 
 * @returns {Promise<void>}
 */
export const deleteBooking = async (id) => {
  try {
    const response = await fetch(`${API_BASE_URL}/posts/${id}`, {
      method: 'DELETE',
    });
    
    if (!response.ok) throw new Error('Failed to delete booking');
  } catch (error) {
    throw new Error(`Delete error: ${error.message}`);
  }
};

/**
 * Gets unique filter options from bookings
 * @param {Array} bookings 
 * @returns {Object}
 */
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