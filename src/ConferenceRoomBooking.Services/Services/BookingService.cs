using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Services.Models;

namespace ConferenceRoomBooking.Services.Services
{
    /// <summary>
    /// Service responsible for managing conference room bookings
    /// Implements business logic using C# collections and LINQ
    /// </summary>
    public class BookingService
    {
        // Collections as specified in assignment requirements
        private readonly List<ConferenceRoom> _conferenceRooms;
        private readonly List<Booking> _bookings;
        
        // Dictionary for fast room lookup by ID
        private readonly Dictionary<int, ConferenceRoom> _roomsById;
        
        // Counter for booking IDs
        private int _nextBookingId = 1;
        
        /// <summary>
        /// Initializes a new instance of the BookingService
        /// </summary>
        public BookingService()
        {
            _conferenceRooms = new List<ConferenceRoom>();
            _bookings = new List<Booking>();
            _roomsById = new Dictionary<int, ConferenceRoom>();
        }
        
        /// <summary>
        /// Gets all conference rooms
        /// </summary>
        public IReadOnlyList<ConferenceRoom> ConferenceRooms => _conferenceRooms.AsReadOnly();
        
        /// <summary>
        /// Gets all bookings
        /// </summary>
        public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();
        
        /// <summary>
        /// Adds a conference room to the system
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <param name="name">Room name</param>
        /// <param name="capacity">Room capacity</param>
        /// <param name="type">Room type</param>
        public void AddConferenceRoom(int id, string name, int capacity, RoomType type)
        {
            if (_roomsById.ContainsKey(id))
                throw new ArgumentException($"Room with ID {id} already exists", nameof(id));
            
            // Use object initializer instead of constructor
            var room = new ConferenceRoom
            {
                Id = id,
                Name = name,
                Capacity = capacity,
                Type = type
            };
            
            _conferenceRooms.Add(room);
            _roomsById[id] = room;
        }
        
        /// <summary>
        /// Adds an existing conference room to the system
        /// </summary>
        public void AddConferenceRoom(ConferenceRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room), "Room cannot be null");
                
            if (_roomsById.ContainsKey(room.Id))
                throw new ArgumentException($"Room with ID {room.Id} already exists", nameof(room));
            
            _conferenceRooms.Add(room);
            _roomsById[room.Id] = room;
        }
        
        /// <summary>
        /// Attempts to create a new booking
        /// Enforces all business rules before creating the booking
        /// </summary>
        /// <param name="request">The booking request</param>
        /// <returns>A BookingResult indicating success or failure</returns>
        public BookingResult CreateBooking(BookingRequest request)
        {
            // BUSINESS RULE 1: Booking must reference an existing conference room
            if (!_roomsById.TryGetValue(request.RoomId, out var room))
            {
                return BookingResult.Failure($"Conference room with ID {request.RoomId} does not exist");
            }
            
            // Validate time range
            if (request.StartTime >= request.EndTime)
            {
                return BookingResult.Failure("Start time must be before end time");
            }
            
            if (request.StartTime < DateTime.Now)
            {
                return BookingResult.Failure("Cannot book rooms in the past");
            }
            
            // BUSINESS RULE 2: A conference room cannot be double-booked for overlapping time slots
            bool isRoomAvailable = !_bookings.Any(existingBooking =>
                existingBooking.RoomId == request.RoomId &&
                existingBooking.Status != BookingStatus.Cancelled &&
                IsTimeOverlapping(
                    existingBooking.StartTime, existingBooking.EndTime,
                    request.StartTime, request.EndTime));
            
            if (!isRoomAvailable)
            {
                return BookingResult.Failure(
                    $"Room {room.Name} is already booked for the requested time slot");
            }
            
            // Create the booking object
            var booking = new Booking
            {
                Id = _nextBookingId++,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                UserName = request.UserName,
                Notes = request.Notes,
                CreatedAt = DateTime.Now
            };
            
            try
            {
                // BUSINESS RULE 3: Booking must move through valid states only
                // Auto-confirm for demo purposes
                booking.Confirm(); // This will set Status and UpdatedAt
            }
            catch (InvalidOperationException ex)
            {
                return BookingResult.Failure($"Failed to confirm booking: {ex.Message}");
            }
            
            _bookings.Add(booking);
            
            return BookingResult.Success(booking);
        }
        
        /// <summary>
        /// Checks if two time ranges overlap
        /// </summary>
        private bool IsTimeOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && end1 > start2;
        }
        
        /// <summary>
        /// Gets all bookings for a specific room
        /// </summary>
        /// <param name="roomId">The room ID</param>
        /// <returns>List of bookings for the room</returns>
        public List<Booking> GetBookingsForRoom(int roomId)
        {
            return _bookings
                .Where(b => b.RoomId == roomId)
                .OrderBy(b => b.StartTime)
                .ToList();
        }
        
        /// <summary>
        /// Gets all available rooms for a given time slot
        /// </summary>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>List of available rooms</returns>
        public List<ConferenceRoom> GetAvailableRooms(DateTime startTime, DateTime endTime)
        {
            // Get IDs of rooms that are booked during this time
            var bookedRoomIds = _bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                           IsTimeOverlapping(b.StartTime, b.EndTime, startTime, endTime))
                .Select(b => b.RoomId)
                .ToHashSet();
            
            // Return all rooms that are not booked
            return _conferenceRooms
                .Where(room => !bookedRoomIds.Contains(room.Id))
                .ToList();
        }
        
        /// <summary>
        /// BUSINESS RULE 4: Invalid booking requests must be rejected early (fail-fast)
        /// This method demonstrates early validation
        /// </summary>
        public (bool isValid, string errorMessage) ValidateBookingRequest(BookingRequest request)
        {
            if (request == null)
                return (false, "Booking request cannot be null");
                
            if (request.StartTime >= request.EndTime)
                return (false, "Start time must be before end time");
                
            if (request.StartTime < DateTime.Now)
                return (false, "Cannot book rooms in the past");
                
            if (string.IsNullOrWhiteSpace(request.UserName))
                return (false, "User name is required");
                
            if (!_roomsById.ContainsKey(request.RoomId))
                return (false, $"Room with ID {request.RoomId} does not exist");
                
            return (true, string.Empty);
        }
        
        /// <summary>
        /// Finds a conference room by ID
        /// </summary>
        public ConferenceRoom? GetRoomById(int roomId)
        {
            return _roomsById.TryGetValue(roomId, out var room) ? room : null;
        }
        
        /// <summary>
        /// Gets a booking by ID
        /// </summary>
        public Booking? GetBookingById(int bookingId)
        {
            return _bookings.FirstOrDefault(b => b.Id == bookingId);
        }
        
        /// <summary>
        /// Cancels a booking by ID
        /// </summary>
        public bool CancelBooking(int bookingId)
        {
            var booking = GetBookingById(bookingId);
            if (booking == null)
                return false;
            
            try
            {
                booking.Cancel();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Demonstrates LINQ Any() method for business logic
        /// </summary>
        public bool HasAnyBookingsForRoom(int roomId)
        {
            return _bookings.Any(b => b.RoomId == roomId);
        }
        
        /// <summary>
        /// Demonstrates LINQ All() method for business logic
        /// </summary>
        public bool AreAllRoomsAvailable(DateTime startTime, DateTime endTime)
        {
            return _conferenceRooms.All(room => 
                !_bookings.Any(b => 
                    b.RoomId == room.Id && 
                    b.Status != BookingStatus.Cancelled &&
                    IsTimeOverlapping(b.StartTime, b.EndTime, startTime, endTime)));
        }
        
        /// <summary>
        /// Demonstrates LINQ FirstOrDefault() method for business logic
        /// </summary>
        public Booking? FindBookingByUserName(string userName)
        {
            return _bookings.FirstOrDefault(b => 
                b.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Demonstrates LINQ Where() method for filtering collections
        /// </summary>
        public List<Booking> GetUpcomingBookings()
        {
            return _bookings
                .Where(b => b.StartTime > DateTime.Now && b.Status != BookingStatus.Cancelled)
                .OrderBy(b => b.StartTime)
                .ToList();
        }
        
        /// <summary>
        /// Demonstrates LINQ Count() method
        /// </summary>
        public int GetBookingCountForUser(string userName)
        {
            return _bookings.Count(b => 
                b.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
