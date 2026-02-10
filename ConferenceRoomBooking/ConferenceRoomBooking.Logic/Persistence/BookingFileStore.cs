using System.Text.Json;
using ConferenceRoomBooking.Domain;

namespace ConferenceRoomBooking.Logic.Persistence;

public class BookingFileStore : IBookingStore
{
    private readonly string _filePath;

    public BookingFileStore(string folder)
    {
        Directory.CreateDirectory(folder);
        _filePath = Path.Combine(folder, "bookings.json");
    }

    public async Task SaveAsync(IEnumerable<Booking> bookings)
    {
        var json = JsonSerializer.Serialize(bookings);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<Booking>> LoadAsync()
    {
        if (!File.Exists(_filePath))
            return new List<Booking>();

        var json = await File.ReadAllTextAsync(_filePath);

        return JsonSerializer.Deserialize<List<Booking>>(json)
               ?? new List<Booking>();
    }
}
