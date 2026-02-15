using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BookingSystem
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.1, 3.4 - Room Manager (Business Logic Layer)
    /// 
    /// 🎓 WHAT IS THIS CLASS?
    /// This manager handles all business rules and operations for Conference Rooms.
    /// It sits between the controllers (API) and the data store.
    /// 
    /// 🎓 RESPONSIBILITIES:
    /// ✅ Enforce business rules (unique room numbers)
    /// ✅ Coordinate room operations (create, delete)
    /// ✅ Work with domain entities (ConferenceRoom)
    /// ❌ Currently using in-memory list instead of database!
    /// 
    /// 🎓 LAYER ARCHITECTURE:
    /// ┌─────────────────┐
    /// │ RoomController  │ ← HTTP, validation, responses
    /// ├─────────────────┤
    /// │  RoomManager    │ ← YOU ARE HERE! Business rules
    /// ├─────────────────┤
    /// │  EFBookingStore │ ← Data access (should be used!)
    /// └─────────────────┘
    /// </summary>
    public class RoomManager     //All business rules
    {
        // 🚨 CRITICAL ISSUE: In-memory storage!
        // This List<> stores rooms in MEMORY, not in DATABASE.
        // When the app restarts, ALL ROOM DATA IS LOST!
        // 
        // 📌 ASSIGNMENT 3.1 - This should be replaced with IBookingStore
        private readonly List<ConferenceRoom> _rooms;

        /// <summary>
        /// 🎓 Constructor - creates a new RoomManager
        /// 
        /// 📌 ISSUE: No dependency injection
        /// Should accept IBookingStore to work with database
        /// 
        /// ✅ BETTER:
        /// private readonly IBookingStore _store;
        /// public RoomManager(IBookingStore store)
        /// {
        ///     _store = store;
        /// }
        /// </summary>
        public RoomManager()
        {
            _rooms = new List<ConferenceRoom>();
        }
        
        /// <summary>
        /// 📌 Get all rooms
        /// 
        /// ⚠️ Returns data from MEMORY, not database
        /// Also returns ALL rooms (no filtering, no pagination)
        /// 
        /// 📌 ASSIGNMENT 3.3 - Should support:
        /// - Filtering by location/capacity/active status
        /// - Pagination
        /// - Sorting
        /// - Only active rooms for regular users (Assignment 3.4)
        /// </summary>
        public IReadOnlyList<ConferenceRoom> GetRooms()
        {
            return _rooms.ToList();  // Returns a copy of the list
        }

        /// <summary>
        /// 📌 Create a new room
        /// 
        /// 🎓 BUSINESS RULES ENFORCED:
        /// 1. ✅ Room number must exist (not null)
        /// 2. ✅ Room number must be unique (no duplicates)
        /// 
        /// 🎓 BUSINESS RULES MISSING:
        /// ❌ Capacity validation (should be positive)
        /// ❌ Location validation (should be provided)
        /// ❌ Room type validation (should be valid enum)
        /// ❌ ID should NOT be set by client!
        /// 
        /// 📌 ASSIGNMENT 3.4 - Data Integrity:
        /// These rules ensure room data stays clean
        /// 
        /// ⚠️⚠️⚠️ CRITICAL ISSUES:
        /// 
        /// 1. ❌ Takes ID from client! Client could set any ID:
        ///    request.Room.ID = 999  // Could overwrite existing room!
        /// 
        /// 2. ❌ No capacity validation:
        ///    request.Room.Capacity = -5  // Would create negative capacity!
        /// 
        /// 3. ❌ No location validation:
        ///    request.Room.location = null  // Would create room with no location!
        /// 
        /// 4. ❌ No room type validation:
        ///    request.Room.type = (RoomType)999  // Invalid enum value!
        /// </summary>
        public ConferenceRoom CreateRoom(RoomRequest request)
        {
            // 🎓 Rule: Room number must not be null
            if(request.Room.RoomNumber == null)
            {
                throw new ArgumentException("Room must exist");
            }
            
            // 🎓 Rule: Room number must be unique
            bool overlaps = _rooms.Any(b => b.RoomNumber == request.Room.RoomNumber);

            if (overlaps)
            {
                throw new ArgumentException("Room Number already used");
            }

            // 🚨 PROBLEM: Using client-provided ID!
            // Client could send ID=999 and overwrite existing room!
            ConferenceRoom room = new ConferenceRoom(
                request.Room.ID,              // ❌ Don't trust client ID!
                request.Room.RoomNumber,
                request.Room.Capacity,         // ❌ No validation!
                request.Room.type               // ❌ No validation!
            );

            // 🚨 Storing in MEMORY - will be lost on restart!
            _rooms.Add(room);

            return room;
        }

        /// <summary>
        /// 📌 Delete a room (HARD DELETE)
        /// 
        /// ⚠️⚠️⚠️ CRITICAL ISSUES:
        /// 
        /// 1. ❌ HARD DELETE - completely removes room from list!
        ///    Should be SOFT DELETE (mark IsActive = false)
        /// 
        /// 2. ❌ No check for existing bookings!
        ///    Could delete a room that has future bookings
        /// 
        /// 3. ❌ Finds by RoomNumber only (what if duplicates?)
        ///    Should use unique ID
        /// 
        /// 4. ❌ No authorization check
        ///    Should only allow Facilities Manager/Admin
        /// 
        /// 📌 ASSIGNMENT 3.4 - Should be SOFT DELETE:
        /// - Set IsActive = false instead of removing
        /// - Check for future bookings first
        /// - Preserve room data for historical bookings
        /// </summary>
        public bool DeleteRoom(RoomRequest request)
        {
            // 🎓 Validation
            if(request.Room.RoomNumber == null)
            {
                throw new ArgumentException("Room must exist");
            }

            // 🎓 Check if room exists
            bool overlaps = _rooms.Any(b => b.RoomNumber == request.Room.RoomNumber);
            
            if (overlaps)
            {
                // ❌ HARD DELETE - removes the room completely!
                var roomToRemove = _rooms.First(b => b.RoomNumber == request.Room.RoomNumber);
                _rooms.Remove(roomToRemove);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - ROOM MANAGER ANALYSIS:
/// 
/// 📌 WHAT'S WORKING WELL:
/// ✅ Unique room number enforcement
/// ✅ Basic validation for room number
/// ✅ Clean separation from controllers
/// 
/// 📌 CRITICAL ISSUES TO FIX:
/// 
/// 1️⃣ PERSISTENCE (Assignment 3.1)
///    ❌ Using List<ConferenceRoom> in memory
///    ✅ Should inject IBookingStore and use database
/// 
/// 2️⃣ HARD DELETE vs SOFT DELETE (Assignment 3.4)
///    ❌ DeleteRoom does HARD DELETE (removes record)
///    ✅ Should set IsActive = false instead
///    ✅ Should check for future bookings first
/// 
/// 3️⃣ ID TRUST ISSUE (Security)
///    ❌ Accepts ID from client in CreateRoom
///    ✅ Should ignore client ID, let database generate
/// 
/// 4️⃣ MISSING VALIDATION
///    ❌ No capacity validation (should be positive)
///    ❌ No location validation
///    ❌ No room type validation
/// 
/// 5️⃣ MISSING AUTHORIZATION (Assignment 2.4)
///    ❌ No user context
///    ✅ Should check roles (Facilities Manager only)
/// 
/// 6️⃣ FINDING BY ROOMNUMBER
///    ❌ Uses RoomNumber to find rooms (not unique if duplicates)
///    ✅ Should use ID for all operations
/// 
/// 🚀 IMPROVED VERSION:
/// 
/// public class RoomManager
/// {
///     private readonly IBookingStore _store;
///     
///     public RoomManager(IBookingStore store)
///     {
///         _store = store;
///     }
///     
///     public async Task<ConferenceRoom> CreateRoomAsync(CreateRoomDto dto)
///     {
///         // Validate
///         if (string.IsNullOrWhiteSpace(dto.RoomNumber))
///             throw new ArgumentException("Room number required");
///         
///         if (dto.Capacity <= 0)
///             throw new ArgumentException("Capacity must be positive");
///         
///         if (string.IsNullOrWhiteSpace(dto.Location))
///             throw new ArgumentException("Location required");
///         
///         // Check uniqueness
///         var existing = await _store.GetRoomByNumberAsync(dto.RoomNumber);
///         if (existing != null)
///             throw new ArgumentException($"Room {dto.RoomNumber} already exists");
///         
///         // Create new room (ID not set - database will generate)
///         var room = new ConferenceRoom(
///             roomNumber: dto.RoomNumber,
///             capacity: dto.Capacity,
///             type: dto.Type,
///             location: dto.Location
///         );
///         
///         await _store.SaveRoomAsync(room);
///         return room;
///     }
///     
///     public async Task<bool> SoftDeleteRoomAsync(int roomId)
///     {
///         // Check for future bookings
///         var hasFutureBookings = await _store.RoomHasFutureBookingsAsync(roomId);
///         if (hasFutureBookings)
///             throw new InvalidOperationException("Cannot delete room with future bookings");
///         
///         // Soft delete
///         return await _store.DeactivateRoomAsync(roomId);
///     }
///     
///     public async Task<List<ConferenceRoom>> GetActiveRoomsAsync()
///     {
///         return await _store.GetActiveRoomsAsync();  // Only IsActive = true
///     }
/// }
/// </summary>