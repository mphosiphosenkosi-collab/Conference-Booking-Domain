using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Logic.Interfaces;

public interface IRoomService
{
    Task<ConferenceRoom> CreateRoomAsync(string name, int capacity, string roomType);
    Task<ConferenceRoom> GetRoomByIdAsync(int id);
    Task<ConferenceRoom> GetRoomByNameAsync(string name);
    Task<IEnumerable<ConferenceRoom>> GetAllRoomsAsync();
    Task<bool> DeleteRoomAsync(int id);
}
