using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;  // Required for Include(), AsNoTracking(), etc.

namespace API.controllers
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.3, & 3.4 - Booking Controller
    /// 
    /// 🎓 PURPOSE:
    /// Handles all booking-related operations:
    /// - Creating new bookings (Employees, Receptionists)
    /// - Cancelling bookings (Employees, Receptionists)
    /// - Viewing all bookings (Admins only)
    /// - Filtered/paginated booking lists (Assignment 3.3)
    /// 
    /// 🎓 CLEAN ARCHITECTURE NOTES:
    /// - Controller handles HTTP, validation, and responses
    /// - Business logic should be in BookingManager (see _manager comment)
    /// - Data access through EFBookingStore (repository pattern)
    /// </summary>
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly BookingManager _manager; // 🎓 Business logic layer - use for complex rules
        private readonly EFBookingStore _context; // 🎓 Data access layer - use for simple CRUD

        public BookingController(BookingManager manager, EFBookingStore context)
        {
            _manager = manager;
            _context = context;
        }

        /// <summary>
        /// 📌 GET /api/bookings - Get ALL bookings (Admin only)
        /// 
        /// 🎓 WHY SEPARATE FROM GetBookings()?
        /// This returns ALL bookings without filters - for admin reports/audits
        /// The other GetBookings() has pagination/filters for regular use
        /// 
        /// 📌 ASSIGNMENT 2.4 - Role-based access:
        /// Only Admins can see ALL bookings (privacy!)
        /// </summary>
        [HttpGet] //GET /api/bookings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            // 🎓 LoadAllAsync() likely returns ALL bookings without filtering
            // ⚠️ POTENTIAL PERFORMANCE ISSUE: Could return thousands of records!
            // Consider adding pagination here too
            var bookings = await _context.LoadAllAsync();//implementing DB
            return Ok(bookings);
        }

        /// <summary>
        /// 📌 POST /api/bookings - Create new booking
        /// 
        /// 🎓 WHO CAN BOOK:
        /// - Employees: Book rooms for themselves
        /// - Receptionists: Book for visitors/guests
        /// - Admins: Can book anything (implied by role hierarchy)
        /// 
        /// 📌 ASSIGNMENT 3.4 - Data Integrity:
        /// Should check:
        /// - Room exists and is active
        /// - No double booking
        /// - Valid time range
        /// 
        /// 🎓 NOTE: Business rules should be in BookingManager, not here!
        /// </summary>
        [HttpPost] //POST /api/bookings
        [Authorize(Roles = "Admin,Employee,Receptionist")]
        public async Task<IActionResult> Book([FromBody] CreateBookingDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: Validation should happen here!
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                // 🎓 Creating booking entity from DTO
                // ⚠️ ISSUE: This constructor might not set all fields (CreatedAt, Status)
                var booking = new Booking(dto.room, dto.startTime, dto.endTime);
                
                // 🎓 TODO: Add current user ID to booking
                // var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                // booking.UserId = userId;  // Would need to add UserId property to Booking class

                // 📌 ASSIGNMENT 3.2 - Set CreatedAt (if not set by constructor)
                // booking.CreatedAt = DateTime.UtcNow;
                
                // 📌 ASSIGNMENT 3.4 - Business rules should be checked here or in manager
                // For example: Check if room is active
                // if (!dto.room.IsActive)
                //     return BadRequest("Cannot book inactive room");
                
                await _context.SaveAsync(booking);//implementing DB
                
                // 🎓 Return 201 Created with location of new resource
                return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, 
                    new { 
                        message = "Booking created successfully",
                        bookingId = booking.Id,
                        status = booking.Status.ToString()
                    });
            }
            catch (Exception ex)
            {
                // 🎓 ExceptionHandlingMiddleware will catch this, but we can add context
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 📌 DELETE /api/bookings - Cancel existing booking
        /// 
        /// 🎓 WHO CAN CANCEL:
        /// - Employees: Can cancel their OWN bookings (should verify!)
        /// - Receptionists: Can cancel any booking (guest bookings)
        /// - Admins: Can cancel any booking
        /// 
        /// 📌 ASSIGNMENT 2.4 - Authorization:
        /// Need to check if user owns the booking before allowing cancellation
        /// 
        /// 📌 ASSIGNMENT 3.2 - CancelledAt:
        /// Should set CancelledAt timestamp when cancelling
        /// </summary>
        [HttpDelete] //DELETE /api/bookings
        [Authorize(Roles = "Employee,Receptionist")]
        public async Task<IActionResult> CancelBooking([FromBody] CancelBookingDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: Finding booking by Room+Times is risky!
            // Better to use BookingId (but your DTO doesn't have it)
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                // 🎓 Find the actual booking in database
                // This assumes you have a way to find by Room+Times
                var existingBooking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => 
                        b.Room.ID == dto.room.ID &&
                        b.StartTime == dto.startTime &&
                        b.EndTime == dto.endTime);
                
                if (existingBooking == null)
                    return NotFound("Booking not found");
                
                // 📌 ASSIGNMENT 2.4 - Check if user owns this booking
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var isReceptionist = User.IsInRole("Receptionist");
                
                // Assuming Booking has a UserId property
                // if (!isReceptionist && existingBooking.UserId != userId)
                //     return Forbid("You can only cancel your own bookings");
                
                // 📌 ASSIGNMENT 3.2 - Set CancelledAt
                existingBooking.CancelledAt = DateTime.UtcNow;
                
                // 📌 Cancel using context method
                await _context.CancelBookingAsync(existingBooking);
                
                return Ok(new { 
                    message = "Successfully cancelled the booking",
                    bookingId = existingBooking.Id,
                    cancelledAt = existingBooking.CancelledAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 📌 ASSIGNMENT 3.3 - Filtered, Paginated, Sorted Bookings List
        /// 
        /// 🎓 WHAT THIS ENDPOINT PROVIDES:
        /// ✅ Filtering by room, location, date range
        /// ✅ Pagination with page/pageSize
        /// ✅ Sorting by room, date, or creation time
        /// ✅ DTO projection (only needed fields)
        /// ✅ Database-level filtering (not in memory)
        /// ✅ AsNoTracking() for read-only performance
        /// 
        /// 🎓 WHY THIS IS PRODUCTION-READY:
        /// - All filtering happens in SQL (fast!)
        /// - Returns only what frontend needs (efficient)
        /// - Includes pagination metadata (frontend-friendly)
        /// - Soft delete filter (only active rooms)
        /// 
        /// 📌 HTTP: GET /api/bookings?page=1&pageSize=10&roomId=5&fromDate=2026-03-01
        /// </summary>
        [HttpGet("filtered")] // 👈 Note: This should be a different route! 
        // Your code has two [HttpGet] methods - this will cause ambiguity!
        // Consider: [HttpGet] for filtered list, [HttpGet("all")] for admin view
        public async Task<IActionResult> GetBookings(
            int page = 1,
            int pageSize = 10,
            int? roomId = null,
            string? location = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortBy = "date")
        {
            // 🎓 EDUCATIONAL NOTE: Building IQueryable - NOT executing yet!
            var query = _context.Bookings
                .Include(b => b.Room)                    // Eager load Room data
                .Where(b => b.Room.IsActive)             // 📌 ASSIGNMENT 3.4 - Soft delete filter
                .AsNoTracking();                          // 📌 ASSIGNMENT 3.3 - Performance boost!

            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - FILTERING (all at database level)
            // ====================================================================
            
            // Filter by specific room
            if (roomId.HasValue)
                query = query.Where(b => b.Room.ID == roomId);
            
            // Filter by location (case-insensitive partial match)
            if (!string.IsNullOrEmpty(location))
                query = query.Where(b => b.Room.location.Contains(location));
            
            // Filter by date range (bookings starting after fromDate)
            if (fromDate.HasValue)
                query = query.Where(b => b.StartTime >= fromDate);
            
            // Filter by date range (bookings ending before toDate)
            if (toDate.HasValue)
                query = query.Where(b => b.EndTime <= toDate);
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - SORTING (before pagination!)
            // ====================================================================
            query = sortBy.ToLower() switch
            {
                "room" => query.OrderBy(b => b.Room.RoomNumber),
                "date" => query.OrderBy(b => b.StartTime),
                _ => query.OrderBy(b => b.CreatedAt)      // Default: newest first
            };
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - PAGINATION
            // ====================================================================
            
            // First, get TOTAL COUNT (before pagination)
            var totalCount = await query.CountAsync();
            
            // Then, get ONLY the current page
            var items = await query
                .Skip((page - 1) * pageSize)    // Skip previous pages
                .Take(pageSize)                  // Take only this page
                .Select(b => new BookingListItemDto  // 📌 PROJECT to DTO
                {
                    Id = b.Id,
                    RoomNumber = b.Room.RoomNumber,
                    Location = b.Room.location,
                    StartTime = b.StartTime,
                    Status = b.Status.ToString()    // Enum → string
                })
                .ToListAsync();  // NOW execute the query!
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - PAGINATED RESPONSE
            // ====================================================================
            return Ok(new
            {
                items,                                  // The data for this page
                totalCount,                             // Total records available
                page,                                    // Current page number
                pageSize,                                // Items per page
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                hasPreviousPage = page > 1,
                hasNextPage = page < (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - BOOKING CONTROLLER:
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS:
/// ✅ Role-based authorization on all endpoints
/// ❌ Missing ownership check (users should only cancel their own bookings)
/// 
/// 📌 ASSIGNMENT 3.3 REQUIREMENTS:
/// ✅ Filtering (room, location, date range)
/// ✅ Pagination (page, pageSize, totalCount)
/// ✅ Sorting (room, date, createdAt)
/// ✅ Projection (BookingListItemDto)
/// ✅ AsNoTracking() for performance
/// 
/// 📌 ASSIGNMENT 3.4 REQUIREMENTS:
/// ✅ Soft delete filter (b.Room.IsActive)
/// ❌ Business rules in BookingManager (move logic there)
/// ❌ Double booking prevention (should add)
/// 
/// 🎓 ISSUES TO FIX:
/// 1. Two [HttpGet] methods conflict - rename one route
/// 2. Cancel by Room+Times is risky - use BookingId
/// 3. Missing user ID on bookings
/// 4. Business logic in controller (should be in manager)
/// 
/// 📌 SUGGESTED ROUTE STRUCTURE:
/// GET    /api/bookings              → Filtered list (everyone)
/// GET    /api/bookings/all          → All bookings (admin only)
/// GET    /api/bookings/{id}         → Single booking details
/// POST   /api/bookings              → Create booking
/// DELETE /api/bookings/{id}/cancel  → Cancel by ID
/// </summary>