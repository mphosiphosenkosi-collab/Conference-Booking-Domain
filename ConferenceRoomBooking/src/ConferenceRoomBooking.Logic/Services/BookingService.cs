using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Exceptions;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Logic.Interfaces;
using ConferenceRoomBooking.Persistence.Repositories;

namespace ConferenceRoomBooking.Logic.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
    }

    public async Task<BookingResult> CreateBookingAsync(BookingRequest request)
    {
        var errors = new List<string>();

        // 1. Validate request
        if (request.StartTime >= request.EndTime)
            errors.Add("Start time must be before end time");
        
        if (request.StartTime < DateTime.UtcNow)
            errors.Add("Start time cannot be in the past");

        if (string.IsNullOrWhiteSpace(request.EmployeeId))
            errors.Add("Employee ID is required");

        if (string.IsNullOrWhiteSpace(request.RoomName))
            errors.Add("Room name is required");

        if (errors.Any())
            return BookingResult.FailureResult("Validation failed", errors);

        try
        {
            // Note: For now, we'll use RoomId = 1 as placeholder
            // In a real app, you'd look up the room by name
            const int roomId = 1;  // TODO: Implement room lookup
            
            // 2. Check for overlaps
            bool hasOverlap = await _bookingRepository.HasOverlapAsync(
                roomId, request.StartTime, request.EndTime);
            
            if (hasOverlap)
                throw new BookingOverlapException($"Room '{request.RoomName}' is already booked for the requested time");

            // 3. Create booking
            var bookings = await _bookingRepository.GetAllAsync();
            var nextId = bookings.Any() ? bookings.Max(b => b.Id) + 1 : 1;
            
            var booking = new Booking(
                nextId,
                request.EmployeeId,
                roomId,
                request.StartTime,
                request.EndTime,
                BookingStatus.Confirmed);

            // 4. Save booking
            var savedBooking = await _bookingRepository.AddAsync(booking);
            
            return BookingResult.SuccessResult(savedBooking, "Booking created successfully");
        }
        catch (BookingOverlapException ex)
        {
            return BookingResult.FailureResult(ex.Message);
        }
        catch (Exception ex)
        {
            return BookingResult.FailureResult($"Internal error: {ex.Message}");
        }
    }

    public Task<Booking?> GetBookingByIdAsync(int id)
        => _bookingRepository.GetByIdAsync(id);

    public Task<IEnumerable<Booking>> GetAllBookingsAsync()
        => _bookingRepository.GetAllAsync();

    public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(string roomName)
    {
        // TODO: Implement room lookup by name
        const int roomId = 1;
        return await _bookingRepository.GetByRoomIdAsync(roomId);
    }

    public async Task<BookingResult> CancelBookingAsync(int id)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return BookingResult.FailureResult($"Booking with ID {id} not found");

            booking.Cancel();
            await _bookingRepository.UpdateAsync(booking);
            
            return BookingResult.SuccessResult(booking, "Booking cancelled successfully");
        }
        catch (Exception ex)
        {
            return BookingResult.FailureResult($"Failed to cancel booking: {ex.Message}");
        }
    }

    public async Task<bool> CheckRoomAvailabilityAsync(string roomName, DateTime start, DateTime end)
    {
        // TODO: Implement room lookup by name
        const int roomId = 1;
        return !await _bookingRepository.HasOverlapAsync(roomId, start, end);
    }

    public Task<IEnumerable<ConferenceRoom>> GetAllRoomsAsync()
    {
        // TODO: Implement room repository
        return Task.FromResult(Enumerable.Empty<ConferenceRoom>());
    }
}
