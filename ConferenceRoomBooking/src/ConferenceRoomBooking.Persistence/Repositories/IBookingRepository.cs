using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Persistence.Repositories;

public interface IBookingRepository
{
    Task<Booking> GetByIdAsync(int id);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking> AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
    Task DeleteAsync(int id);
    Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end, int? excludeBookingId = null);
    Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId);
    Task<IEnumerable<Booking>> GetByEmployeeIdAsync(string employeeId);
}
