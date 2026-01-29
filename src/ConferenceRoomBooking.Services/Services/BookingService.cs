using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Services.Exceptions;
using ConferenceRoomBooking.Services.Models;

namespace ConferenceRoomBooking.Services.Services
{
    public class BookingService
    {
        private readonly List<ConferenceRoom> _conferenceRooms;
        private readonly List<Booking> _bookings;
        private readonly Dictionary<int, ConferenceRoom> _roomsById;
        private int _nextBookingId = 1;
        private readonly IDataService _dataService;
        
        public BookingService(IDataService? dataService = null)
        {
            _conferenceRooms = new List<ConferenceRoom>();
            _bookings = new List<Booking>();
            _roomsById = new Dictionary<int, ConferenceRoom>();
            _dataService = dataService ?? new JsonDataService();
        }
        
        public IReadOnlyList<ConferenceRoom> ConferenceRooms => _conferenceRooms.AsReadOnly();
        public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();
        
        public void AddConferenceRoom(int id, string name, int capacity, RoomType type)
        {
            if (id <= 0)
                throw new ArgumentException("Room ID must be positive", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Room name cannot be null or empty", nameof(name));
            if (capacity <= 0)
                throw new ArgumentException("Room capacity must be positive", nameof(capacity));
            if (_roomsById.ContainsKey(id))
                throw new ArgumentException($"Room with ID {id} already exists", nameof(id));
            
            var room = new ConferenceRoom { Id = id, Name = name, Capacity = capacity, Type = type };
            _conferenceRooms.Add(room);
            _roomsById[id] = room;
        }
        
        public void AddConferenceRoom(ConferenceRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room), "Room cannot be null");
            if (room.Id <= 0)
                throw new ArgumentException("Room ID must be positive", nameof(room));
            if (string.IsNullOrWhiteSpace(room.Name))
                throw new ArgumentException("Room name cannot be null or empty", nameof(room));
            if (room.Capacity <= 0)
                throw new ArgumentException("Room capacity must be positive", nameof(room));
            if (_roomsById.ContainsKey(room.Id))
                throw new ArgumentException($"Room with ID {room.Id} already exists", nameof(room));
            
            _conferenceRooms.Add(room);
            _roomsById[room.Id] = room;
        }
        
        public BookingResult CreateBooking(BookingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Booking request cannot be null");
            if (string.IsNullOrWhiteSpace(request.UserName))
                throw new InvalidBookingException("User name is required");
            if (request.StartTime >= request.EndTime)
                throw new InvalidBookingException("Start time must be before end time");
            if (request.StartTime < DateTime.Now)
                throw new InvalidBookingException("Cannot book rooms in the past");
            
            if (!_roomsById.TryGetValue(request.RoomId, out var room))
                throw new RoomNotFoundException(request.RoomId);
            
            var conflictingBooking = _bookings
                .Where(b => b.RoomId == request.RoomId && b.Status != BookingStatus.Cancelled)
                .FirstOrDefault(b => IsTimeOverlapping(b.StartTime, b.EndTime, request.StartTime, request.EndTime));
            
            if (conflictingBooking != null)
            {
                throw new BookingConflictException(
                    $"Room '{room.Name}' is already booked from " +
                    $"{conflictingBooking.StartTime:yyyy-MM-dd HH:mm} to " +
                    $"{conflictingBooking.EndTime:HH:mm} by {conflictingBooking.UserName}");
            }
            
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
                booking.Confirm();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidBookingException($"Failed to confirm booking: {ex.Message}", ex);
            }
            
            _bookings.Add(booking);
            return BookingResult.Success(booking);
        }
        
        private bool IsTimeOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && end1 > start2;
        }
        
        public List<Booking> GetBookingsForRoom(int roomId)
        {
            if (!_bookings.Any())
                return new List<Booking>();
            if (!_roomsById.ContainsKey(roomId))
                throw new RoomNotFoundException(roomId);
            
            return _bookings
                .Where(b => b.RoomId == roomId)
                .OrderBy(b => b.StartTime)
                .ToList();
        }
        
        public List<ConferenceRoom> GetAvailableRooms(DateTime startTime, DateTime endTime)
        {
            if (startTime >= endTime)
                throw new InvalidBookingException("Start time must be before end time");
            if (!_conferenceRooms.Any())
                return new List<ConferenceRoom>();
            
            var bookedRoomIds = _bookings
                .Where(b => b != null && b.Status != BookingStatus.Cancelled)
                .Where(b => IsTimeOverlapping(b.StartTime, b.EndTime, startTime, endTime))
                .Select(b => b.RoomId)
                .Distinct()
                .ToList();
            
            return _conferenceRooms
                .Where(room => room != null && !bookedRoomIds.Contains(room.Id))
                .ToList();
        }
        
        public (bool isValid, string errorMessage) ValidateBookingRequest(BookingRequest request)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
                return (false, "An error occurred during validation");
            }
        }
        
        public ConferenceRoom? GetRoomById(int roomId)
        {
            return _roomsById.TryGetValue(roomId, out var room) ? room : null;
        }
        
        public Booking? GetBookingById(int bookingId)
        {
            if (!_bookings.Any())
                return null;
            return _bookings.FirstOrDefault(b => b.Id == bookingId);
        }
        
        public bool CancelBooking(int bookingId)
        {
            if (!_bookings.Any())
                throw new InvalidOperationException("No bookings exist in the system");
            
            var booking = GetBookingById(bookingId);
            if (booking == null)
                throw new BookingNotFoundException(bookingId);
            
            try
            {
                booking.Cancel();
                return true;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidBookingException($"Cannot cancel booking {bookingId}: {ex.Message}", ex);
            }
        }
        
        public bool HasAnyBookingsForRoom(int roomId)
        {
            if (!_bookings.Any())
                return false;
            if (!_roomsById.ContainsKey(roomId))
                throw new RoomNotFoundException(roomId);
            return _bookings.Any(b => b.RoomId == roomId);
        }
        
        public bool AreAllRoomsAvailable(DateTime startTime, DateTime endTime)
        {
            if (!_conferenceRooms.Any())
                return true;
            if (startTime >= endTime)
                throw new InvalidBookingException("Invalid time range");
            
            return _conferenceRooms.All(room => 
                !_bookings.Any(b => 
                    b.RoomId == room.Id && 
                    b.Status != BookingStatus.Cancelled &&
                    IsTimeOverlapping(b.StartTime, b.EndTime, startTime, endTime)));
        }
        
        public Booking? FindBookingByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be null or empty", nameof(userName));
            if (!_bookings.Any())
                return null;
            return _bookings.FirstOrDefault(b => 
                b.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
        
        public List<Booking> GetUpcomingBookings()
        {
            if (!_bookings.Any())
                return new List<Booking>();
            return _bookings
                .Where(b => b != null && b.StartTime > DateTime.Now && b.Status != BookingStatus.Cancelled)
                .OrderBy(b => b.StartTime)
                .ToList();
        }
        
        public int GetBookingCountForUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be null or empty", nameof(userName));
            if (!_bookings.Any())
                return 0;
            return _bookings.Count(b => 
                b.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
        
        public async Task LoadDataFromFileAsync(string filePath = "bookings_data.json")
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            
            try
            {
                var data = await _dataService.LoadFromFileAsync<BookingData>(filePath);
                if (data != null)
                {
                    _conferenceRooms.Clear();
                    _roomsById.Clear();
                    _bookings.Clear();
                    
                    foreach (var room in data.Rooms)
                        if (room != null) AddConferenceRoom(room);
                    
                    foreach (var booking in data.Bookings.Where(b => b != null))
                        _bookings.Add(booking);
                    
                    if (data.Bookings.Any())
                        _nextBookingId = data.Bookings.Max(b => b.Id) + 1;
                    
                    Console.WriteLine($"✅ Loaded {data.Rooms.Count} rooms and {data.Bookings.Count} bookings from {filePath}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"ℹ️  No existing data file found at {filePath}. Starting with empty system.");
            }
            catch (DataAccessException ex)
            {
                throw new DataAccessException($"Failed to load data from {filePath}", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Unexpected error loading from {filePath}", ex);
            }
        }
        
        public async Task SaveDataToFileAsync(string filePath = "bookings_data.json")
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            
            try
            {
                var data = new BookingData
                {
                    Rooms = _conferenceRooms.ToList(),
                    Bookings = _bookings.ToList()
                };
                
                await _dataService.SaveToFileAsync(filePath, data);
                Console.WriteLine($"✅ Saved {_conferenceRooms.Count} rooms and {_bookings.Count} bookings to {filePath}");
            }
            catch (DataAccessException ex)
            {
                throw new DataAccessException($"Failed to save data to {filePath}", ex);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Unexpected error saving to {filePath}", ex);
            }
        }
        
        public void DemonstrateDefensiveProgramming()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║      ASSIGNMENT 1.3: DEFENSIVE PROGRAMMING DEMOS    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            Console.WriteLine("\n1. Testing Guard Clauses:");
            try
            {
                AddConferenceRoom(0, "Test", 10, RoomType.Standard);
                Console.WriteLine("❌ Should have thrown exception for ID 0");
            }
            catch (ArgumentException ex) { Console.WriteLine($"✅ Guard clause caught: {ex.Message}"); }
            
            try
            {
                AddConferenceRoom(1, "", 10, RoomType.Standard);
                Console.WriteLine("❌ Should have thrown exception for empty name");
            }
            catch (ArgumentException ex) { Console.WriteLine($"✅ Guard clause caught: {ex.Message}"); }
            
            Console.WriteLine("\n2. Testing Empty Collections:");
            var tempService = new BookingService();
            Console.WriteLine($"Available rooms from empty service: {tempService.GetAvailableRooms(DateTime.Now, DateTime.Now.AddHours(1)).Count} (should be 0)");
            Console.WriteLine($"Upcoming bookings from empty service: {tempService.GetUpcomingBookings().Count} (should be 0)");
            
            Console.WriteLine("\n3. Testing Safe LINQ:");
            Console.WriteLine($"Non-existent booking result: {(GetBookingById(999) == null ? "null ✅" : "found ❌")}");
            
            Console.WriteLine("\n4. Testing Custom Exceptions:");
            try
            {
                CancelBooking(999);
                Console.WriteLine("❌ Should have thrown BookingNotFoundException");
            }
            catch (BookingNotFoundException ex) { Console.WriteLine($"✅ Custom exception caught: {ex.Message}"); }
            catch (InvalidOperationException ex) { Console.WriteLine($"✅ Caught empty collection: {ex.Message}"); }
            
            Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║           ALL A1.3 DEMOS COMPLETED                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        }
    }
}
