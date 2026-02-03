using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Logic.Interfaces;

public interface IBookingService
{
    Task<BookingResult> CreateBookingAsync(BookingRequest request);
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
    Task<IEnumerable<Booking>> GetBookingsByRoomAsync(string roomName);
    Task<BookingResult> CancelBookingAsync(int id);
    Task<bool> CheckRoomAvailabilityAsync(string roomName, DateTime start, DateTime end);
    Task<IEnumerable<ConferenceRoom>> GetAllRoomsAsync();
}
