using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Persistence.Stores;
using ConferenceRoomBooking.Logic.Interfaces;

namespace ConferenceRoomBooking.Persistence.Repositories;

public class BookingRepository : IBookingRepository  // Now references Logic.Interfaces
{
    private readonly JsonDataStore _dataStore;

    public BookingRepository(JsonDataStore dataStore)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    }

    public Task<Booking?> GetByIdAsync(int id)
    {
        var booking = _dataStore.GetBookings().FirstOrDefault(b => b.Id == id);
        return Task.FromResult(booking);
    }

    public Task<IEnumerable<Booking>> GetAllAsync()
    {
        var bookings = _dataStore.GetBookings().AsEnumerable();
        return Task.FromResult(bookings);
    }

    public Task<Booking> AddAsync(Booking booking)
    {
        _dataStore.AddBooking(booking);
        return Task.FromResult(booking);
    }

    public Task UpdateAsync(Booking booking)
    {
        _dataStore.UpdateBooking(booking);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _dataStore.RemoveBooking(id);
        return Task.CompletedTask;
    }

    public Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeBookingId = null)
    {
        var bookings = _dataStore.GetBookings()
            .Where(b => b.RoomId == roomId && b.Status != Domain.Enums.BookingStatus.Cancelled);
        
        if (excludeBookingId.HasValue)
        {
            bookings = bookings.Where(b => b.Id != excludeBookingId.Value);
        }

        var hasOverlap = bookings.Any(b => 
            (start >= b.StartTime && start < b.EndTime) ||
            (end > b.StartTime && end <= b.EndTime) ||
            (start <= b.StartTime && end >= b.EndTime));
        
        return Task.FromResult(hasOverlap);
    }

    public Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId)
    {
        var bookings = _dataStore.GetBookings()
            .Where(b => b.RoomId == roomId)
            .AsEnumerable();
        
        return Task.FromResult(bookings);
    }

    public Task<IEnumerable<Booking>> GetByEmployeeIdAsync(string employeeId)
    {
        var bookings = _dataStore.GetBookings()
            .Where(b => b.EmployeeId == employeeId)
            .AsEnumerable();
        
        return Task.FromResult(bookings);
    }
}
