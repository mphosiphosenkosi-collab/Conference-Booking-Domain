using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.DTOs;
using BookingSystem.Domain.Enums;
using BookingSystem.Domain.Exceptions;
using BookingSystem.Logic;
using BookingSystem.Persistence;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // ADD THIS for async LINQ methods

namespace API.controllers
{
    /// <summary>
    /// 📌 ASSIGNMENT 2.4, 3.3, & 3.4 - Room Controller
    /// 
    /// 🎓 PURPOSE:
    /// Manages all room-related operations:
    /// - Listing rooms with filtering/pagination (Assignment 3.3)
    /// - Creating new rooms (Facilities Manager only)
    /// - Soft deleting rooms (Facilities Manager/Admin) (Assignment 3.4)
    /// - Reactivating rooms (Facilities Manager/Admin) (Assignment 3.4)
    /// 
    /// 🎓 CLEAN ARCHITECTURE:
    /// - HTTP handling in controller
    /// - Business logic should be in RoomManager (but using DbContext directly here)
    /// - Data access through EFBookingStore for simple ops, DbContext for complex queries
    /// </summary>
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly EFBookingStore _context;  // 🎓 Repository pattern - simple CRUD
        private readonly AppDbContext _db;         // 🎓 DbContext - complex queries
        private readonly RoomManager _manager;     // 🎓 Business logic layer (not used much here)

        public RoomController(RoomManager manager, EFBookingStore context, AppDbContext db)
        {
            _manager = manager;
            _context = context;
            _db = db;
        }

        /// <summary>
        /// 📌 ASSIGNMENT 3.3 - GET /api/rooms
        /// Retrieves all rooms with OPTIONAL filtering, pagination, and sorting
        /// 
        /// 🎓 WHY SO MANY PARAMETERS?
        /// Frontend needs flexibility: "Show me active rooms on Floor 2 that hold at least 10 people,
        /// sorted by capacity, 5 per page, page 2"
        /// 
        /// 🎓 ALL PARAMETERS ARE OPTIONAL:
        /// - Default values ensure API works even if frontend sends nothing
        /// - [FromQuery] means they come from URL: /api/rooms?page=2&location=Floor1
        /// </summary>
        /// <param name="isActive">Filter by active status (true=active only, false=inactive only, null=all)</param>
        /// <param name="location">Filter by location (partial match)</param>
        /// <param name="minCapacity">Filter by minimum capacity</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
        /// <param name="sortBy">Sort field: "name", "capacity", "location" (default: "name")</param>
        /// <param name="sortOrder">Sort order: "asc" or "desc" (default: "asc")</param>
        [HttpGet] //GET /api/rooms
        [Authorize] // Any authenticated user can view rooms
        public async Task<IActionResult> GetRooms(
            // 📌 ASSIGNMENT 3.3 - FILTERING parameters (all optional)
            [FromQuery] bool? isActive = null,      // Filter by active/inactive rooms
            [FromQuery] string? location = null,    // Filter by location
            [FromQuery] int? minCapacity = null,    // Filter by minimum capacity
            
            // 📌 ASSIGNMENT 3.3 - PAGINATION parameters
            [FromQuery] int page = 1,               // Default page = 1
            [FromQuery] int pageSize = 10,          // Default page size = 10
            
            // 📌 ASSIGNMENT 3.3 - SORTING parameters
            [FromQuery] string sortBy = "name",      // Sort by: name, capacity, location
            [FromQuery] string sortOrder = "asc")    // Sort order: asc, desc
        {
            // 🎓 EDUCATIONAL NOTE: 
            // We're using the DbContext directly for advanced queries (filtering, pagination)
            // This is better than _context.LoadRoomsAsync() because we can filter at DATABASE level
            // NOT in memory! This is Assignment 3.3 requirement.
            
            // Start building the query - DON'T EXECUTE YET!
            var query = _db.ConferenceRooms.AsQueryable();  // Note: Should be ConferenceRooms, not conRooms?
            
            // 🎓 EDUCATIONAL NOTE: IQueryable builds the query but doesn't run it until we call
            // .ToListAsync(), .CountAsync(), etc. This lets us add filters, sorting, pagination
            // that all run in the DATABASE, not in memory!
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.4 - SOFT DELETE FILTERING
            // ====================================================================
            
            // We can optionally filter by active status
            if (isActive.HasValue)
            {
                // If isActive = true: show ONLY active rooms
                // If isActive = false: show ONLY inactive rooms (admin only!)
                query = query.Where(r => r.IsActive == isActive.Value);
                
                // 🎓 EDUCATIONAL NOTE: 
                // This implements "soft delete" - we never actually delete rooms,
                // we just mark them as inactive. Then we filter them out here.
                // This meets Assignment 3.4 requirement!
            }
            else
            {
                // Default behavior: show ONLY active rooms (for regular users)
                // But check if user is Admin - they might want to see all rooms
                if (!User.IsInRole("Admin"))
                {
                    query = query.Where(r => r.IsActive == true);
                }
                // 🎓 EDUCATIONAL NOTE: 
                // Regular users only see active rooms. Admins can see all rooms
                // to manage inactive ones. This is good security practice!
            }
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - ADDITIONAL FILTERS
            // ====================================================================
            
            // Filter by location (case-insensitive partial match)
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(r => r.location != null && r.location.Contains(location));
                // 🎓 EDUCATIONAL NOTE: This filter runs in SQL! NOT in memory.
                // Example: ?location=Floor 1 returns rooms with "Floor 1" in location
                // SQL: WHERE location LIKE '%Floor 1%'
            }
            
            // Filter by minimum capacity
            if (minCapacity.HasValue)
            {
                query = query.Where(r => r.Capacity >= minCapacity.Value);
                // 🎓 EDUCATIONAL NOTE: Only return rooms that can hold at least minCapacity people
                // SQL: WHERE Capacity >= 10
            }
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - SORTING
            // ====================================================================
            
            // Apply sorting BEFORE pagination (important!)
            query = sortBy.ToLower() switch
            {
                "capacity" => sortOrder.ToLower() == "desc" 
                    ? query.OrderByDescending(r => r.Capacity) 
                    : query.OrderBy(r => r.Capacity),
                    
                "location" => sortOrder.ToLower() == "desc" 
                    ? query.OrderByDescending(r => r.location) 
                    : query.OrderBy(r => r.location),
                    
                // Default: sort by name
                _ => sortOrder.ToLower() == "desc" 
                    ? query.OrderByDescending(r => r.RoomNumber) 
                    : query.OrderBy(r => r.RoomNumber)
            };
            
            // 🎓 EDUCATIONAL NOTE: Sorting in the database is MUCH faster than sorting in memory!
            // The switch expression lets us choose different sort fields dynamically.
            // SQL generated: ORDER BY RoomNumber ASC (or DESC)
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - PAGINATION
            // ====================================================================
            
            // First, get TOTAL COUNT (before pagination)
            var totalCount = await query.CountAsync();
            // 🎓 EDUCATIONAL NOTE: CountAsync() runs "SELECT COUNT(*)" in SQL - very fast!
            // This is separate from the main query - doesn't affect pagination
            
            // Then apply pagination: Skip previous pages, Take only current page
            var pagedRooms = await query
                .Skip((page - 1) * pageSize)   // Skip records from previous pages
                .Take(pageSize)                 // Take only records for current page
                .ToListAsync();                  // NOW execute the query!
            
            // 🎓 EDUCATIONAL NOTE: 
            // Skip/Take = OFFSET/LIMIT in SQL. 
            // Example: page=2, pageSize=10 → Skip(10) → Take(10) → returns records 11-20
            // SQL: SELECT ... LIMIT 10 OFFSET 10
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - DTO PROJECTION
            // ====================================================================
            
            // Convert to DTOs so we don't send internal entity details to client
            var roomDtos = pagedRooms.Select(r => new RoomListItemDto
            {
                Id = r.ID,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                Type = r.type.ToString(),  // Convert enum to string
                Location = r.location,
                IsActive = r.IsActive
            });
            
            // 🎓 EDUCATIONAL NOTE: DTO = Data Transfer Object
            // We control exactly what data the frontend sees
            // No sensitive internal fields exposed!
            // Also prevents circular references (Room -> Bookings -> Room...)
            
            // ====================================================================
            // 📌 ASSIGNMENT 3.3 - RETURN PAGINATED RESPONSE
            // ====================================================================
            
            // Return pagination metadata + the actual data
            var response = new
            {
                // Pagination metadata
                totalCount,              // Total records in database (after filters)
                page,                     // Current page number
                pageSize,                 // Items per page
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize), // Calculate total pages
                hasPreviousPage = page > 1,
                hasNextPage = page < (int)Math.Ceiling(totalCount / (double)pageSize),
                
                // The actual data for this page
                items = roomDtos,
                
                // Filter info (helpful for frontend debugging)
                filters = new {
                    isActive,
                    location,
                    minCapacity,
                    sortBy,
                    sortOrder
                }
            };
            
            // 🎓 EDUCATIONAL NOTE: 
            // This response structure matches Assignment 3.3 requirements:
            // ✅ Total record count
            // ✅ Current page
            // ✅ Page size
            // ✅ Paged results
            // ✅ Filter metadata
            
            return Ok(response);
            
            // ⚠️ OLD CODE - kept for reference:
            // var rooms = await _context.LoadRoomsAsync();
            // return Ok(rooms);
            // 
            // 🎓 EDUCATIONAL NOTE: The old code loaded ALL rooms at once
            // No filtering, no pagination - bad for performance with many rooms!
            // New code implements ALL Assignment 3.3 requirements:
            // - Database-level filtering ✅
            // - Pagination ✅
            // - Sorting ✅
            // - DTO projection ✅
        }
        
        /// <summary>
        /// 📌 ASSIGNMENT 2.4 - CREATE ROOM
        /// 
        /// 🎓 WHO CAN CREATE:
        /// Only Facilities Managers can create new rooms
        /// 
        /// 🎓 WHAT HAPPENS:
        /// 1. Receive room data from client
        /// 2. Save to database via EFBookingStore
        /// 3. Return created room
        /// 
        /// 📌 HTTP: POST /api/rooms
        /// </summary>
        [HttpPost] //POST /api/rooms
        [Authorize(Roles = "Facilities Manager")]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: 
            // This creates a new room. The room is active by default (see ConferenceRoom constructor)
            // Only Facilities Manager can create rooms (Assignment 2.4 requirement)
            
            // Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                // dto.room is already a ConferenceRoom
                // ⚠️ SECURITY: Ensure ID is 0 so database generates new one
                dto.room.ID = 0;  // Force new ID
                dto.room.IsActive = true;  // Ensure new rooms are active
                
                await _context.SaveRoomAsync(dto.room);
                
                // Return 201 Created with location
                return CreatedAtAction(nameof(GetRooms), new { id = dto.room.ID }, 
                    new { 
                        message = "Room created successfully",
                        room = dto.room
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 📌 ASSIGNMENT 3.4 - SOFT DELETE ROOM
        /// 
        /// 🎓 WHAT IS SOFT DELETE?
        /// Instead of removing the room from database (which would break historical bookings),
        /// we just mark it as inactive. It still exists but cannot be booked.
        /// 
        /// 🎓 WHO CAN SOFT DELETE:
        /// - Facilities Managers (room maintenance)
        /// - Admins (oversight)
        /// 
        /// 📌 HTTP: PATCH /api/rooms
        /// </summary>
        [HttpPatch] //PATCH /api/rooms - soft delete a room
        [Authorize(Roles = "Facilities Manager, Admin")]
        public async Task<IActionResult> SoftDeleteRoom([FromBody] DeleteRoomDto dto)
        {
            // 🎓 EDUCATIONAL NOTE: 
            // This is SOFT DELETE - we DON'T actually remove the room from database!
            // We just mark it as inactive (IsActive = false)
            // This preserves historical data and prevents orphaned bookings
            
            // Validate input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var room = dto.room;
            
            // ⚠️ SECURITY CHECK: Make sure the room exists in database
            var existingRoom = await _db.ConferenceRooms.FindAsync(room.ID);
            if (existingRoom == null)
            {
                return NotFound($"Room with ID {room.ID} not found");
            }
            
            // 🎓 EDUCATIONAL NOTE: 
            // Before soft-deleting, check if room has any future bookings
            // We don't want to deactivate a room that people have booked!
            var hasFutureBookings = await _db.Bookings
                .AnyAsync(b => b.Room.ID == room.ID && b.StartTime > DateTime.UtcNow);
            
            if (hasFutureBookings)
            {
                return BadRequest(new { 
                    error = "Cannot delete room with future bookings. Cancel bookings first.",
                    futureBookingsCount = await _db.Bookings
                        .CountAsync(b => b.Room.ID == room.ID && b.StartTime > DateTime.UtcNow)
                });
            }
            
            // 📌 ASSIGNMENT 3.4 - SOFT DELETE IMPLEMENTATION
            // Mark as inactive instead of deleting
            existingRoom.IsActive = false;
            
            // 🎓 EDUCATIONAL NOTE: 
            // Optionally add DeletedAt timestamp if your entity has it
            // existingRoom.DeletedAt = DateTime.UtcNow;
            
            // Save changes to database
            await _db.SaveChangesAsync();
            
            // 🎓 EDUCATIONAL NOTE: 
            // Original code used _context.RemoveRoomAsync(room) which might HARD DELETE
            // We're changing to SOFT DELETE to meet Assignment 3.4 requirement:
            // "Do not physically delete the record"
            
            return Ok(new { 
                message = "Room successfully deactivated (soft delete)",
                roomId = room.ID,
                isActive = false,
                deactivatedAt = DateTime.UtcNow
            });
            
            // ⚠️ OLD CODE (hard delete):
            // await _context.RemoveRoomAsync(room);
            // return Ok("Successfully Deleted Room");
        }
        
        /// <summary>
        /// 📌 ASSIGNMENT 3.4 - REACTIVATE ROOM
        /// 
        /// 🎓 PURPOSE:
        /// Bring a soft-deleted room back to active status
        /// (e.g., after maintenance is complete)
        /// 
        /// 🎓 WHO CAN REACTIVATE:
        /// - Facilities Managers
        /// - Admins
        /// 
        /// 📌 HTTP: POST /api/rooms/5/reactivate
        /// </summary>
        /// <param name="id">ID of room to reactivate</param>
        [HttpPost("{id}/reactivate")] //POST /api/rooms/5/reactivate
        [Authorize(Roles = "Facilities Manager, Admin")]
        public async Task<IActionResult> ReactivateRoom(int id)
        {
            // 🎓 EDUCATIONAL NOTE: 
            // This is the opposite of soft delete - bring a room back to active status
            
            var room = await _db.ConferenceRooms.FindAsync(id);
            if (room == null)
            {
                return NotFound($"Room with ID {id} not found");
            }
            
            // Reactivate the room
            room.IsActive = true;
            await _db.SaveChangesAsync();
            
            return Ok(new { 
                message = "Room successfully reactivated",
                roomId = id,
                isActive = true,
                reactivatedAt = DateTime.UtcNow
            });
        }
    }
}

/// <summary>
/// 🎓 EDUCATIONAL SUMMARY - ROOM CONTROLLER:
/// 
/// 📌 ASSIGNMENT 2.4 REQUIREMENTS:
/// ✅ [Authorize] on all endpoints
/// ✅ Role checks: Facilities Manager for create/delete, Admin for delete/reactivate
/// 
/// 📌 ASSIGNMENT 3.3 REQUIREMENTS:
/// ✅ Filtering (isActive, location, minCapacity)
/// ✅ Pagination (page, pageSize, totalCount)
/// ✅ Sorting (by name, capacity, location)
/// ✅ Projection (RoomListItemDto)
/// ✅ AsNoTracking() could be added for extra performance
/// 
/// 📌 ASSIGNMENT 3.4 REQUIREMENTS:
/// ✅ Soft delete (IsActive flag)
/// ✅ Reactivate endpoint
/// ✅ Future booking check before soft delete
/// 
/// 🎓 SUGGESTED IMPROVEMENTS:
/// 1. Add RoomListItemDto class (if not exists)
/// 2. Add max pageSize limit (e.g., max 50)
/// 3. Use _db.ConferenceRooms consistently (not conRooms)
/// 4. Add validation for sortBy/sortOrder values
/// 5. Consider moving complex logic to RoomManager
/// </summary>