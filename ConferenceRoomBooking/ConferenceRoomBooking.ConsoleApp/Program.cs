using System;
using System.IO;
using System.Threading.Tasks;
using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Logic;

// If you still have RoomType ambiguity, keep this alias:
using DomainRoomType = ConferenceRoomBooking.Domain.RoomType;

namespace ConferenceRoomBooking.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Assignment 1.3 Robust Booking Demo ===\n");

            var manager = new BookingManager();

            // -----------------------------
            // Create Rooms
            // -----------------------------
            manager.AddRoom(new ConferenceRoom(
                1,
                "Executive Boardroom",
                DomainRoomType.Boardroom,
                12,
                new[] { "Projector", "Whiteboard" }
            ));

            // -----------------------------
            // VALID BOOKING
            // -----------------------------
            try
            {
                var booking = manager.TryCreateBooking(
                    1001,
                    1,
                    "john@company.com",
                    DateTime.Now.AddHours(1),
                    DateTime.Now.AddHours(2));

                booking.Confirm();

                Console.WriteLine($"✅ Booking created and confirmed: #{booking.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            }

            // -----------------------------
            // OVERLAP FAILURE (custom exception)
            // -----------------------------
            try
            {
                manager.TryCreateBooking(
                    1002,
                    1,
                    "jane@company.com",
                    DateTime.Now.AddMinutes(90),
                    DateTime.Now.AddHours(2.5));
            }
            catch (BookingConflictException ex)
            {
                Console.WriteLine($"⚠️ Expected conflict handled: {ex.Message}");
            }

            // -----------------------------
            // INVALID ROOM FAILURE
            // -----------------------------
            try
            {
                manager.TryCreateBooking(
                    1003,
                    99,
                    "ghost@company.com",
                    DateTime.Now.AddHours(3),
                    DateTime.Now.AddHours(4));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Expected invalid room error: {ex.Message}");
            }

            // -----------------------------
            // ASYNC FILE SAVE / LOAD
            // -----------------------------
            var path = Path.Combine(Environment.CurrentDirectory, "bookings.json");

            try
            {
                await manager.SaveBookingsAsync(path);
                Console.WriteLine("✅ Async save successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Save failed: {ex.Message}");
            }

            try
            {
                await manager.LoadBookingsAsync(path);
                Console.WriteLine("✅ Async load successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Load failed: {ex.Message}");
            }

            // -----------------------------
            // INTENTIONAL LOAD FAILURE
            // -----------------------------
            try
            {
                await manager.LoadBookingsAsync("missing-file.json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Expected async failure handled: {ex.Message}");
            }

            // -----------------------------
            // LIST BOOKINGS SAFELY
            // -----------------------------
            Console.WriteLine("\nCurrent bookings:");

            var all = manager.GetAllBookings();

            if (!all.Any())
            {
                Console.WriteLine("No bookings found.");
            }
            else
            {
                foreach (var b in all)
                {
                    Console.WriteLine(
                        $"Booking #{b.Id} | Room {b.RoomId} | {b.StartTime:t}-{b.EndTime:t} | {b.Status}");
                }
            }

            Console.WriteLine("\n=== Assignment 1.3 Demo Complete ===");
            Console.ReadKey();
        }
    }
}
