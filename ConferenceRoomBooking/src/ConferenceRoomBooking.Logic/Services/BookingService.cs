using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Exceptions;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Logic.Interfaces;


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

        // 1. Validate request with SPECIFIC ERROR CODES (for HTTP status mapping)

        // Business rule validations - return specific error codes (will become 422)
        if (request.StartTime >= request.EndTime)
            return BookingResult.InvalidTimeRange(); // This maps to 422

        if (request.StartTime < DateTime.UtcNow.AddMinutes(-5)) // Allow 5-minute buffer
            return BookingResult.PastStartTime(); // This maps to 422

        // Basic field validations - these are validation errors (will become 400)
        if (string.IsNullOrWhiteSpace(request.EmployeeId))
            errors.Add("Employee ID is required");

        if (string.IsNullOrWhiteSpace(request.RoomName))
            errors.Add("Room name is required");

        if (errors.Any())
            return BookingResult.FailureResult("Validation failed", "VALIDATION_ERROR", errors);

        try
        {
            // Note: For now, we'll use RoomId = 1 as placeholder
            const int roomId = 1;  // TODO: Implement room lookup

            // 2. Check for overlaps
            bool hasOverlap = await _bookingRepository.HasOverlapAsync(
                roomId, request.StartTime, request.EndTime);

            if (hasOverlap)
                return BookingResult.RoomUnavailable(request.RoomName, request.StartTime, request.EndTime); // This maps to 422

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
            // If we still get this exception somehow, convert to proper result
            return BookingResult.RoomUnavailable(request.RoomName, request.StartTime, request.EndTime);
        }
        catch (Exception ex)
        {
            // Log the full exception for debugging
           // _logger.LogError(ex, "Unexpected error creating booking");

            return BookingResult.FailureResult(
                "An unexpected error occurred while creating the booking",
                "INTERNAL_ERROR",
                new List<string> { ex.Message });
        }
    }

    public Task<Booking?> GetBookingByIdAsync(int id)
        => _bookingRepository.GetByIdAsync(id);

    public Task<IEnumerable<Booking>> GetAllBookingsAsync()
        => _bookingRepository.GetAllAsync();

    public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(string roomName)
    {
        const int roomId = 1;
        return await _bookingRepository.GetByRoomIdAsync(roomId);
    }

    public async Task<BookingResult> CancelBookingAsync(int id)
    {
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return BookingResult.FailureResult($"Booking with ID {id} not found", "NOT_FOUND");

            booking.Cancel();
            await _bookingRepository.UpdateAsync(booking);

            return BookingResult.SuccessResult(booking, "Booking cancelled successfully");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot cancel"))
        {
            return BookingResult.FailureResult(
                $"Cannot cancel booking with current status: {ex.Message}",
                "INVALID_STATUS",
                new List<string> { ex.Message });
        }
        catch (Exception ex)
        {
            return BookingResult.FailureResult(
                $"Failed to cancel booking: {ex.Message}",
                "INTERNAL_ERROR");
        }
    }

    public async Task<bool> CheckRoomAvailabilityAsync(string roomName, DateTime start, DateTime end)
    {
        const int roomId = 1;
        return !await _bookingRepository.HasOverlapAsync(roomId, start, end);
    }

    public Task<IEnumerable<ConferenceRoom>> GetAllRoomsAsync()
    {
        return Task.FromResult(Enumerable.Empty<ConferenceRoom>());
    }
}