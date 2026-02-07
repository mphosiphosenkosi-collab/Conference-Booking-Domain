// File: BookingManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceRoomBooking.Domain;
using System.Text.Json;
using System.IO;

namespace ConferenceRoomBooking.Logic
{
    public class BookingManager
    {
        private readonly List<ConferenceRoom> _rooms = new();
        private readonly List<Booking> _bookings = new();

        public void AddRoom(ConferenceRoom room)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            if (_rooms.Any(r => r.Id == room.Id))
                throw new ArgumentException($"Room with ID {room.Id} already exists.");

            _rooms.Add(room);
        }

        public Booking TryCreateBooking(int id, int roomId, string userEmail, DateTime start, DateTime end)
        {
            if (!_rooms.Any()) throw new InvalidOperationException("No conference rooms available.");

            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null) throw new ArgumentException($"Room ID {roomId} does not exist.");

            var overlap = _bookings.Any(b => b.RoomId == roomId &&
                                             ((start < b.EndTime) && (end > b.StartTime)));
            if (overlap) throw new BookingConflictException($"Booking conflicts with an existing booking in room {roomId}.");

            var booking = new Booking(id, roomId, userEmail, start, end);
            _bookings.Add(booking);
            return booking;
        }

        public IEnumerable<Booking> GetAllBookings() => _bookings.ToList();
        public IEnumerable<ConferenceRoom> GetAllRooms() => _rooms.ToList();

        public async Task SaveBookingsAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", nameof(filePath));

            try
            {
                var json = JsonSerializer.Serialize(_bookings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(filePath, json);
            }
            catch (IOException ex)
            {
                throw new IOException("Failed to save bookings file.", ex);
            }
        }


        public async Task LoadBookingsAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Booking file not found.", filePath);

            try
            {
                var json = await File.ReadAllTextAsync(filePath);

                var loaded = JsonSerializer.Deserialize<List<Booking>>(json);

                if (loaded == null)
                    throw new InvalidDataException("Booking file contained no data.");

                _bookings.Clear();
                _bookings.AddRange(loaded);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("Booking file format is invalid.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("Failed to load bookings from file.", ex);
            }
        }

    }
}
