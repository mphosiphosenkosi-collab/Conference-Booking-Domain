using System.Text.Json;
using System.IO;
using ConferenceRoomBooking.Domain.Entities;

namespace ConferenceRoomBooking.Persistence.Stores
{
    /// <summary>
    /// Handles storing and loading bookings from a JSON file
    /// </summary>
    public class JsonDataStore
    {
        private readonly string _directoryPath;
        private readonly string _filePath;

        public JsonDataStore(string directoryPath)
        {
            _directoryPath = directoryPath ?? throw new ArgumentNullException(nameof(directoryPath));
            _filePath = Path.Combine(_directoryPath, "bookings.json");
        }

        /// <summary>
        /// Saves the list of bookings to JSON file
        /// </summary>
        public async Task SaveAsync(List<Booking> bookings)
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            string json = JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        /// <summary>
        /// Loads bookings from JSON file
        /// </summary>
        public async Task<List<Booking>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new List<Booking>();

            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
        }

        /// <summary>
        /// Returns the current list of bookings in memory
        /// </summary>
        public List<Booking> GetBookings()
        {
            // Load synchronously for repository simplicity
            return LoadAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Adds a new booking
        /// </summary>
        public void AddBooking(Booking booking)
        {
            var bookings = GetBookings();
            bookings.Add(booking);
            SaveAsync(bookings).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        public void UpdateBooking(Booking booking)
        {
            var bookings = GetBookings();
            var index = bookings.FindIndex(b => b.Id == booking.Id);
            if (index >= 0)
            {
                bookings[index] = booking;
                SaveAsync(bookings).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Removes a booking by Id
        /// </summary>
        public void RemoveBooking(int id)
        {
            var bookings = GetBookings();
            bookings.RemoveAll(b => b.Id == id);
            SaveAsync(bookings).GetAwaiter().GetResult();
        }
    }
}
