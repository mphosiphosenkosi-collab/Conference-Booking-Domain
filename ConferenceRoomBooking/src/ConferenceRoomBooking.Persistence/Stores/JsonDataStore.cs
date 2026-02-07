using System.Text.Json;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Persistence.Stores;

public class JsonDataStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private List<Booking> _bookings;
    private List<ConferenceRoom> _rooms;
    private readonly object _lock = new();

    public JsonDataStore(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        
        // Ensure directory exists
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _bookings = new List<Booking>();
        _rooms = new List<ConferenceRoom>();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                var initialData = new JsonData
                {
                    Bookings = new List<Booking>(),
                    Rooms = new List<ConferenceRoom>()
                };
                SaveToFile(initialData);
                return;
            }

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "{}")
            {
                return;
            }

            var data = JsonSerializer.Deserialize<JsonData>(json, _jsonOptions);
            _bookings = data?.Bookings ?? new List<Booking>();
            _rooms = data?.Rooms ?? new List<ConferenceRoom>();
        }
        catch (JsonException)
        {
            _bookings = new List<Booking>();
            _rooms = new List<ConferenceRoom>();
        }
    }

    private void SaveToFile(object data)
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    }

    // Booking operations
    public List<Booking> GetBookings() => _bookings.ToList();
    public void AddBooking(Booking booking)
    {
        _bookings.Add(booking);
        SaveData();
    }
    
    public void UpdateBooking(Booking updatedBooking)
    {
        var index = _bookings.FindIndex(b => b.Id == updatedBooking.Id);
        if (index >= 0)
        {
            _bookings[index] = updatedBooking;
            SaveData();
        }
    }
    
    public void RemoveBooking(int id)
    {
        _bookings.RemoveAll(b => b.Id == id);
        SaveData();
    }

    // Room operations
    public List<ConferenceRoom> GetRooms() => _rooms.ToList();
    public void AddRoom(ConferenceRoom room)
    {
        _rooms.Add(room);
        SaveData();
    }
    
    public void RemoveRoom(int id)
    {
        _rooms.RemoveAll(r => r.Id == id);  // Now uses Id property
        SaveData();
    }

    private void SaveData()
    {
        var data = new JsonData
        {
            Bookings = _bookings,
            Rooms = _rooms
        };
        SaveToFile(data);
    }

    private class JsonData
    {
        public List<Booking> Bookings { get; set; } = new();
        public List<ConferenceRoom> Rooms { get; set; } = new();
    }
}
