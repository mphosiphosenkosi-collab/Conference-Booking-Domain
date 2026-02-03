using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Logic.Interfaces;

public interface IRoomRepository
{
    Task<ConferenceRoom> GetByIdAsync(int id);
    Task<ConferenceRoom> GetByNameAsync(string name);
    Task<IEnumerable<ConferenceRoom>> GetAllAsync();
    Task<ConferenceRoom> AddAsync(ConferenceRoom room);
    Task UpdateAsync(ConferenceRoom room);
    Task DeleteAsync(int id);
}
