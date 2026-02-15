using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace BookingSystem
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.1, 3.4 - Booking Manager (Business Logic Layer)
    /// 
    /// 🎓 WHAT IS A MANAGER CLASS?
    /// A manager class contains BUSINESS LOGIC - the rules and operations of your system.
    /// It sits between the controllers (API) and the data store (database/repository).
    /// 
    /// 🎓 LAYER ARCHITECTURE:
    /// ┌─────────────────┐
    /// │  Controllers    │ ← HTTP, validation, responses
    /// ├─────────────────┤
    /// │  BookingManager │ ← YOU ARE HERE! Business rules, workflows
    /// ├─────────────────┤
    /// │  EFBookingStore │ ← Data access (database)
    /// └─────────────────┘
    /// 
    /// 🎓 RESPONSIBILITIES:
    /// ✅ Enforce business rules (no double-booking, valid times)
    /// ✅ Coordinate operations (create booking, check conflicts, confirm)
    /// ✅ Work with domain entities (Booking, ConferenceRoom)
    /// ❌ Currently using in-memory list instead of database!
    /// </summary>
    public class BookingManager    
    {
        /// <summary>
        /// 🚨 CRITICAL ISSUE: In-memory storage!
        /// 
        /// This List<> stores bookings in MEMORY, not in DATABASE.
        /// When the app restarts, ALL DATA IS LOST!
        /// 
        /// 📌 ASSIGNMENT 3.1 - This should be replaced with EFBookingStore
        /// 
        /// ❌ Current: _bookings = new List<Booking>()
        /// ✅ Should be: private readonly IBookingStore _store;
        /// </summary>
        private readonly List<Booking> _bookings;

        /// <summary>
        /// 🎓 Constructor - creates a new BookingManager
        /// 
        /// 📌 ISSUE: No dependency injection
        /// Should accept IBookingStore to work with database
        /// 
        /// ✅ BETTER:
        /// public BookingManager(IBookingStore store)
        /// {
        ///     _store = store;
        /// }
        /// </summary>
        public BookingManager()
        {
            _bookings = new List<Booking>();
        }

        /// <summary>
        /// 📌 Get all bookings
        /// 
        /// ⚠️ Returns data from MEMORY, not database
        /// Also returns ALL bookings (no filtering, no pagination)
        /// 
        /// 📌 ASSIGNMENT 3.3 - Should support:
        /// - Filtering by date/room/status
        /// - Pagination
        /// - Sorting
        /// </summary>
        public IReadOnlyList<Booking> GetBookings()
        {
            return _bookings.ToList();  // Returns a copy of the list
        }

        /// <summary>
        /// 📌 Create a new booking (with business rules)
        /// 
        /// 🎓 BUSINESS RULES ENFORCED:
        /// 1. ✅ Room must exist (not null)
        /// 2. ✅ Valid time range (start < end)
        /// 3. ✅ No double-booking (conflict check)
        /// 4. ✅ Booking auto-confirmed (calls Confirm())
        /// 
        /// 📌 ASSIGNMENT 3.4 - Data Integrity:
        /// These rules ensure data stays clean and consistent
        /// 
        /// 🎓 FLOW:
        /// 1. Validate request
        /// 2. Check for conflicts
        /// 3. Create booking
        /// 4. Confirm it
        /// 5. Add to list (should save to DB!)
        /// 6. Return booking
        /// </summary>
        public Booking CreateBooking(BookingRequest request)
        {
            // 🎓 Rule 1: Room must exist
            if(request.Room == null)
            {
                throw new ArgumentException("Room must exist");
            }
            
            // 🎓 Rule 2: Valid time range
            if(request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("Invalid time range");
            }
            
            // 🎓 Rule 3: No double-booking
            // Check if any CONFIRMED booking overlaps with requested time
            bool overlaps = _bookings.Any(b => 
                b.Room == request.Room && 
                b.Status == BookingStatus.Confirmed && 
                request.StartTime < b.EndTime && 
                request.EndTime > b.StartTime);

            if (overlaps)
            {
                // 📌 ASSIGNMENT 2.3 - Domain exception
                throw new BookingConflictException();
            }

            // 🎓 Create the booking
            Booking booking = new Booking(request.Room, request.StartTime, request.EndTime);

            // 🎓 Auto-confirm (business rule: all new bookings are confirmed?)
            // ⚠️ Should new bookings be Pending by default?
            booking.Confirm();
            
            // 🚨 Storing in MEMORY - will be lost on restart!
            _bookings.Add(booking);

            return booking;
        }

        /// <summary>
        /// 📌 Cancel an existing booking
        /// 
        /// 🎓 BUSINESS RULES:
        /// 1. Find matching booking (by Room + Time)
        /// 2. Remove it from list
        /// 
        /// ⚠️⚠️⚠️ CRITICAL ISSUES:
        /// 
        /// 1. ❌ HARD DELETE - removes booking completely (no history)
        ///    Should be SOFT DELETE (mark as Cancelled)
        /// 
        /// 2. ❌ Finds by Room+Time - what if two identical bookings?
        ///    Should use BookingId
        /// 
        /// 3. ❌ No authorization check - anyone can cancel any booking!
        ///    Should check if user owns this booking
        /// 
        /// 4. ❌ No status check - can cancel already-cancelled bookings?
        /// 
        /// 📌 ASSIGNMENT 3.2 - Should set CancelledAt timestamp
        /// 📌 ASSIGNMENT 3.4 - Should update status, not delete
        /// </summary>
        public bool CancelBooking(BookingRequest request)
        {
            // 🎓 Validation
            if(request.Room == null)
            {
                throw new ArgumentException("Room must exist");
            }
            if(request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("Invalid time range");
            }

            // 🎓 Find the booking to cancel
            bool overlaps = _bookings.Any(b => 
                b.Room == request.Room && 
                b.Status == BookingStatus.Confirmed && 
                request.StartTime < b.EndTime && 
                request.EndTime > b.StartTime);
            
            if (overlaps)
            {
                // ❌ HARD DELETE - removes the booking completely!
                var bookingToRemove = _bookings.First(b => 
                    b.Room == request.Room && 
                    b.Status == BookingStatus.Confirmed && 
                    request.StartTime < b.EndTime && 
                    request.EndTime > b.StartTime);
                
                _bookings.Remove(bookingToRemove);
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
/// 🎓 EDUCATIONAL SUMMARY - BOOKING MANAGER ANALYSIS:
/// 
/// 📌 WHAT'S WORKING WELL:
/// ✅ Business rules enforced (no double-booking, valid times)
/// ✅ Domain exceptions used (BookingConflictException)
/// ✅ Clean separation from controllers
/// 
/// 📌 CRITICAL ISSUES TO FIX:
/// 
/// 1️⃣ PERSISTENCE (Assignment 3.1)
///    ❌ Using List<Booking> in memory
///    ✅ Should inject IBookingStore and use database
/// 
/// 2️⃣ SOFT DELETE (Assignment 3.2, 3.4)
///    ❌ CancelBooking does HARD DELETE (removes record)
///    ✅ Should update Status = Cancelled and set CancelledAt
/// 
/// 3️⃣ AUTHORIZATION (Assignment 2.4)
///    ❌ No user context - anyone can cancel any booking
///    ✅ Should accept userId and check ownership
/// 
/// 4️⃣ BOOKING STATUS
///    ❌ CreateBooking auto-confirms (should be Pending)
///    ✅ Should default to Pending, require approval
/// 
/// 5️⃣ BOOKING IDENTIFICATION
///    ❌ Finds bookings by Room+Time (ambiguous)
///    ✅ Should use unique BookingId
/// 
/// 🚀 IMPROVED VERSION:
/// 
/// public class BookingManager
/// {
///     private readonly IBookingStore _store;
///     
///     public BookingManager(IBookingStore store)
///     {
///         _store = store;
///     }
///     
///     public async Task<Booking> CreateBookingAsync(CreateBookingDto dto, string userId)
///     {
///         // Validate
///         if (dto.StartTime >= dto.EndTime)
///             throw new ArgumentException("Invalid time range");
///         
///         // Get fresh room data from DB
///         var room = await _store.GetRoomAsync(dto.RoomId);
///         if (room == null)
///             throw new ArgumentException("Room not found");
///         
///         if (!room.IsActive)
///             throw new InvalidOperationException("Room is not active");
///         
///         // Check for conflicts
///         var conflicts = await _store.GetConflictingBookingsAsync(
///             dto.RoomId, dto.StartTime, dto.EndTime);
///         
///         if (conflicts.Any())
///             throw new BookingConflictException();
///         
///         // Create booking (PENDING status)
///         var booking = new Booking(room, dto.StartTime, dto.EndTime, userId);
///         // Status = Pending by default
///         
///         await _store.SaveBookingAsync(booking);
///         return booking;
///     }
///     
///     public async Task<bool> CancelBookingAsync(int bookingId, string userId)
///     {
///         var booking = await _store.GetBookingAsync(bookingId);
///         if (booking == null)
///             return false;
///         
///         // Check ownership (unless Admin)
///         if (booking.UserId != userId)
///             throw new UnauthorizedAccessException("Not your booking");
///         
///         // Soft delete
///         booking.Cancel();  // Sets Status = Cancelled, CancelledAt = now
///         await _store.UpdateBookingAsync(booking);
///         return true;
///     }
/// }
/// </summary>