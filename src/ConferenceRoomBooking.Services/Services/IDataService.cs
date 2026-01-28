using System.Threading.Tasks;

namespace ConferenceRoomBooking.Services.Services
{
    public interface IDataService
    {
        Task SaveToFileAsync<T>(string filePath, T data);
        Task<T?> LoadFromFileAsync<T>(string filePath);
    }
}