      // For ConferenceRoom, Booking; // For RoomType enum
using ConferenceRoomBooking.Logic;      // For BookingManager
using ConferenceRoomBooking.Domain;     // For BookingConflictException
using System;                             // For DateTime, Console
using System.Collections.Generic;         // For List<>
using System.Threading.Tasks;
using System.IO;                           // For File I/O
using System.Text.Json;                   // For JSON serialization


namespace ConferenceRoomBooking.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new BookingManager();

            SeedRooms(manager);

            Console.WriteLine("=== Conference Room Booking System ===");

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n1. View Rooms");
                Console.WriteLine("2. Create Booking");
                Console.WriteLine("3. View Bookings");
                Console.WriteLine("4. Save Bookings");
                Console.WriteLine("5. Load Bookings");
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ShowRooms(manager);
                            break;

                        case "2":
                            CreateBookingFlow(manager);
                            break;

                        case "3":
                            ShowBookings(manager);
                            break;

                        case "4":
                            manager.SaveBookingsAsync("bookings.json").Wait();
                            Console.WriteLine("Bookings saved.");
                            break;

                        case "5":
                            manager.LoadBookingsAsync("bookings.json").Wait();
                            Console.WriteLine("Bookings loaded.");
                            break;

                        case "0":
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                }
            }
        }
        static void SeedRooms(BookingManager manager)
        {
            manager.AddRoom(new ConferenceRoom(
                1, "Huddle A", Domain.RoomType.Small, 6,
                new List<string> { "TV", "Whiteboard" }));

            manager.AddRoom(new ConferenceRoom(
                2, "Team Room", Domain.RoomType.Medium, 15,
                new List<string> { "Projector", "Whiteboard" }));

            manager.AddRoom(new ConferenceRoom(
                3, "Main Conference", Domain.RoomType.Conference, 80,
                new List<string> { "Stage", "Sound", "Recording" }));
        }


        static void ShowRooms(BookingManager manager)
        {
            Console.WriteLine("\nAvailable Rooms:");
            foreach (var r in manager.GetAllRooms())
            {
                Console.WriteLine($"{r.Id} — {r.Name} — {r.Type} — Capacity {r.Capacity}");
            }
        }

        static void ShowBookings(BookingManager manager)
        {
            Console.WriteLine("\nBookings:");
            foreach (var b in manager.GetAllBookings())
            {
                Console.WriteLine(
                    $"#{b.Id} Room {b.RoomId} | {b.UserEmail} | {b.StartTime} → {b.EndTime}");
            }
        }

        static void CreateBookingFlow(BookingManager manager)
        {
            Console.Write("Booking ID: ");
            int id = int.Parse(Console.ReadLine()!);

            Console.Write("Room ID: ");
            int roomId = int.Parse(Console.ReadLine()!);

            Console.Write("User Email: ");
            string email = Console.ReadLine()!;

            Console.Write("Start (yyyy-MM-dd HH:mm): ");
            DateTime start = DateTime.Parse(Console.ReadLine()!);

            Console.Write("End (yyyy-MM-dd HH:mm): ");
            DateTime end = DateTime.Parse(Console.ReadLine()!);

            var booking = manager.TryCreateBooking(id, roomId, email, start, end);

            Console.WriteLine($"Booking created successfully → #{booking.Id}");
        }
    }
}
