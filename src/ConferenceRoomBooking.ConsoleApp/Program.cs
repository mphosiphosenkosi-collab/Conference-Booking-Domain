using System;
using System.Linq;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Services.Models;
using ConferenceRoomBooking.Services.Services;

namespace ConferenceRoomBooking.ConsoleApp
{
    class Program
    {
        private static BookingService _bookingService = new BookingService();
        
        static void Main(string[] args)
        {
            Console.Title = "Conference Room Booking System";
            
            InitializeSampleData();
            
            bool exitRequested = false;
            
            while (!exitRequested)
            {
                DisplayMainMenu();
                var choice = GetMenuChoice(1, 7);
                
                Console.Clear();
                
                switch (choice)
                {
                    case 1:
                        ViewAllRooms();
                        break;
                    case 2:
                        CreateNewBooking();
                        break;
                    case 3:
                        ViewAllBookings();
                        break;
                    case 4:
                        CancelBooking();
                        break;
                    case 5:
                        CheckRoomAvailability();
                        break;
                    case 6:
                        ViewStatistics();
                        break;
                    case 7:
                        exitRequested = true;
                        Console.WriteLine("\nThank you for using the Conference Room Booking System!");
                        break;
                }
                
                if (!exitRequested)
                {
                    Console.WriteLine("\nPress any key to return to the main menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
        
        static void InitializeSampleData()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║   CONFERENCE ROOM BOOKING SYSTEM                    ║");
            Console.WriteLine("║   Version 1.2 - Interactive Edition                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine("\nInitializing system with sample data...\n");
            
            // Create sample rooms
            var rooms = new[]
            {
                new ConferenceRoom { Id = 1, Name = "Executive Boardroom", Capacity = 20, Type = RoomType.Executive },
                new ConferenceRoom { Id = 2, Name = "Training Center", Capacity = 50, Type = RoomType.Large },
                new ConferenceRoom { Id = 3, Name = "Huddle Space", Capacity = 6, Type = RoomType.Standard },
                new ConferenceRoom { Id = 4, Name = "Video Conference Suite", Capacity = 12, Type = RoomType.VideoConference },
                new ConferenceRoom { Id = 5, Name = "Innovation Lab", Capacity = 15, Type = RoomType.Standard },
                new ConferenceRoom { Id = 6, Name = "Client Meeting Room", Capacity = 8, Type = RoomType.Executive }
            };
            
            foreach (var room in rooms)
            {
                _bookingService.AddConferenceRoom(room);
            }
            
            Console.WriteLine($"✅ Loaded {rooms.Length} conference rooms");
            Console.WriteLine("✅ System ready for interactive use");
        }
        
        static void DisplayMainMenu()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                MAIN MENU                            ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. View All Conference Rooms                        ║");
            Console.WriteLine("║ 2. Create New Booking                               ║");
            Console.WriteLine("║ 3. View All Bookings                                ║");
            Console.WriteLine("║ 4. Cancel a Booking                                 ║");
            Console.WriteLine("║ 5. Check Room Availability                          ║");
            Console.WriteLine("║ 6. View Statistics                                  ║");
            Console.WriteLine("║ 7. Exit                                             ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.Write("\nEnter your choice (1-7): ");
        }
        
        static int GetMenuChoice(int min, int max)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= min && choice <= max)
                {
                    return choice;
                }
                Console.Write($"Invalid input. Please enter a number between {min} and {max}: ");
            }
        }
        
        static void ViewAllRooms()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      ALL CONFERENCE ROOMS                       ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            
            var rooms = _bookingService.ConferenceRooms;
            
            if (!rooms.Any())
            {
                Console.WriteLine("║ No rooms available in the system.                             ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
                return;
            }
            
            Console.WriteLine("║ ID  Name                      Capacity  Type                 ║");
            Console.WriteLine("╟──────────────────────────────────────────────────────────────╢");
            
            foreach (var room in rooms.OrderBy(r => r.Id))
            {
                Console.WriteLine($"║ {room.Id,-3} {room.Name,-25} {room.Capacity,-8} {room.Type,-20} ║");
            }
            
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"\nTotal rooms: {rooms.Count}");
        }
        
        static void CreateNewBooking()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║               CREATE NEW BOOKING                    ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            // Step 1: Show available rooms
            Console.WriteLine("\nAvailable Rooms:");
            Console.WriteLine("────────────────");
            
            var rooms = _bookingService.ConferenceRooms;
            foreach (var room in rooms.OrderBy(r => r.Id))
            {
                Console.WriteLine($"{room.Id}. {room.Name} (Capacity: {room.Capacity}, Type: {room.Type})");
            }
            
            // Step 2: Get room selection
            Console.Write("\nSelect Room ID: ");
            if (!int.TryParse(Console.ReadLine(), out int roomId) || !rooms.Any(r => r.Id == roomId))
            {
                Console.WriteLine("❌ Invalid room selection.");
                return;
            }
            
            // Step 3: Get user details
            Console.Write("Your Name: ");
            string? userNameInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userNameInput))
            {
                Console.WriteLine("❌ Name is required.");
                return;
            }

            string userName = userNameInput;

            Console.Write("Meeting Purpose/Notes: ");
            string? notesInput = Console.ReadLine();
            string notes = notesInput ?? string.Empty;
            
            
            // Step 4: Get date and time
            DateTime startTime, endTime;
            
            Console.Write("Start Date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("❌ Invalid date format.");
                return;
            }
            
            Console.Write("Start Time (HH:mm): ");
            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan startSpan))
            {
                Console.WriteLine("❌ Invalid time format.");
                return;
            }
            
            Console.Write("Duration in hours (e.g., 1.5): ");
            if (!double.TryParse(Console.ReadLine(), out double duration) || duration <= 0)
            {
                Console.WriteLine("❌ Invalid duration.");
                return;
            }
            
            startTime = date.Add(startSpan);
            endTime = startTime.AddHours(duration);
            
            // Step 5: Validate future booking
            if (startTime < DateTime.Now)
            {
                Console.WriteLine("❌ Cannot book rooms in the past.");
                return;
            }
            
            // Step 6: Create booking request
            var request = new BookingRequest
            {
                RoomId = roomId,
                StartTime = startTime,
                EndTime = endTime,
                UserName = userName,
                Notes = notes
            };
            
            // Step 7: Attempt to create booking
            Console.WriteLine("\nProcessing your booking request...");
            
            var validation = _bookingService.ValidateBookingRequest(request);
            if (!validation.isValid)
            {
                Console.WriteLine($"❌ Validation failed: {validation.errorMessage}");
                return;
            }
            
            var result = _bookingService.CreateBooking(request);
            
            // Step 8: Show result
            Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║              BOOKING RESULT                          ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            
            if (result.IsSuccess)
            {
                Console.WriteLine("║ Status: ✅ BOOKING CONFIRMED                        ║");
                Console.WriteLine($"║ Booking ID: {result.Booking?.Id,-34} ║");
                
                var selectedRoom = rooms.First(r => r.Id == roomId);
                Console.WriteLine($"║ Room: {selectedRoom.Name,-35} ║");
                Console.WriteLine($"║ Time: {startTime:yyyy-MM-dd HH:mm} to {endTime:HH:mm,-13} ║");
                Console.WriteLine($"║ Booker: {userName,-36} ║");
            }
            else
            {
                Console.WriteLine("║ Status: ❌ BOOKING FAILED                           ║");
                Console.WriteLine($"║ Reason: {result.ErrorMessage,-35} ║");
                
                // Suggest alternatives
                var availableRooms = _bookingService.GetAvailableRooms(startTime, endTime);
                if (availableRooms.Any())
                {
                    Console.WriteLine("╠══════════════════════════════════════════════════════╣");
                    Console.WriteLine("║ Suggested Alternatives:                            ║");
                    foreach (var room in availableRooms.Take(3))
                    {
                        Console.WriteLine($"║ • {room.Name} (ID: {room.Id}, Capacity: {room.Capacity}) ║");
                    }
                }
            }
            
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        }
        
        static void ViewAllBookings()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                              ALL BOOKINGS                                   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════╣");
            
            var bookings = _bookingService.Bookings;
            var rooms = _bookingService.ConferenceRooms;
            
            if (!bookings.Any())
            {
                Console.WriteLine("║ No bookings found in the system.                                         ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
                return;
            }
            
            Console.WriteLine("║ ID    Room                    Booker           Date       Time       Status  ║");
            Console.WriteLine("╟──────────────────────────────────────────────────────────────────────────────╢");
            
            foreach (var booking in bookings.OrderByDescending(b => b.StartTime))
            {
                // FIXED: Use RoomId to find the room, not booking.Room
                var room = rooms.FirstOrDefault(r => r.Id == booking.RoomId);
                string roomName = room?.Name ?? $"Room {booking.RoomId}";
                string statusIcon = booking.Status == BookingStatus.Confirmed ? "✅" :
                                  booking.Status == BookingStatus.Cancelled ? "❌" :
                                  booking.Status == BookingStatus.Pending ? "⏳" : "✓";
                
                // FIXED: Use UserName property, not BookerName
                Console.WriteLine($"║ {booking.Id,-4} {roomName,-23} {booking.UserName,-15} " +
                                $"{booking.StartTime:yyyy-MM-dd} {booking.StartTime:HH:mm}-{booking.EndTime:HH:mm} " +
                                $"{statusIcon} {booking.Status,-10} ║");
            }
            
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
            
            // Show summary
            int confirmed = bookings.Count(b => b.Status == BookingStatus.Confirmed);
            int cancelled = bookings.Count(b => b.Status == BookingStatus.Cancelled);
            int pending = bookings.Count(b => b.Status == BookingStatus.Pending);
            
            Console.WriteLine($"\nSummary: Confirmed: {confirmed} | Cancelled: {cancelled} | Pending: {pending}");
        }
        
        static void CancelBooking()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║               CANCEL A BOOKING                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            var bookings = _bookingService.Bookings.Where(b => b.Status == BookingStatus.Confirmed);
            var rooms = _bookingService.ConferenceRooms;
            
            if (!bookings.Any())
            {
                Console.WriteLine("\nNo confirmed bookings available to cancel.");
                return;
            }
            
            Console.WriteLine("\nYour Confirmed Bookings:");
            Console.WriteLine("─────────────────────────");
            
            foreach (var booking in bookings)
            {
                // FIXED: Use RoomId to find room
                var room = rooms.FirstOrDefault(r => r.Id == booking.RoomId);
                Console.WriteLine($"{booking.Id}. {room?.Name ?? $"Room {booking.RoomId}"} - " +
                                $"{booking.StartTime:yyyy-MM-dd HH:mm} to {booking.EndTime:HH:mm} " +
                                $"(Booked by: {booking.UserName})");  // FIXED: UserName not BookerName
            }
            
            Console.Write("\nEnter Booking ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("❌ Invalid booking ID.");
                return;
            }
            
            var bookingToCancel = bookings.FirstOrDefault(b => b.Id == bookingId);
            if (bookingToCancel == null)
            {
                Console.WriteLine($"❌ No confirmed booking found with ID {bookingId}.");
                return;
            }
            
            Console.Write($"Are you sure you want to cancel booking {bookingId}? (yes/no): ");
            string? confirmation = Console.ReadLine()?.ToLower();
            
            if (confirmation == "yes" || confirmation == "y")
            {
                try
                {
                    bookingToCancel.Cancel();
                    Console.WriteLine($"\n✅ Booking {bookingId} has been cancelled successfully.");
                    
                    // FIXED: Use RoomId to find room name
                    var room = rooms.FirstOrDefault(r => r.Id == bookingToCancel.RoomId);
                    Console.WriteLine($"The room '{room?.Name ?? $"Room {bookingToCancel.RoomId}"}' is now available for new bookings.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"❌ Cannot cancel booking: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Cancellation aborted.");
            }
        }
        
        static void CheckRoomAvailability()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║           CHECK ROOM AVAILABILITY                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            // Get date and time
            Console.Write("\nCheck availability for date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("❌ Invalid date format.");
                return;
            }
            
            Console.Write("Start Time (HH:mm): ");
            if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan startSpan))
            {
                Console.WriteLine("❌ Invalid time format.");
                return;
            }
            
            Console.Write("Duration in hours (e.g., 2): ");
            if (!double.TryParse(Console.ReadLine(), out double duration) || duration <= 0)
            {
                Console.WriteLine("❌ Invalid duration.");
                return;
            }
            
            DateTime startTime = date.Add(startSpan);
            DateTime endTime = startTime.AddHours(duration);
            
            if (startTime < DateTime.Now)
            {
                Console.WriteLine("⚠️  Note: You're checking availability in the past.");
            }
            
            var availableRooms = _bookingService.GetAvailableRooms(startTime, endTime);
            
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║ Available Rooms from {startTime:HH:mm} to {endTime:HH:mm} on {date:yyyy-MM-dd} ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════════════════╣");
            
            if (!availableRooms.Any())
            {
                Console.WriteLine("║ No rooms available for the selected time slot.                ║");
            }
            else
            {
                Console.WriteLine("║ ID  Name                      Capacity  Type            Status ║");
                Console.WriteLine("╟──────────────────────────────────────────────────────────────╢");
                
                foreach (var room in availableRooms.OrderBy(r => r.Id))
                {
                    Console.WriteLine($"║ {room.Id,-3} {room.Name,-25} {room.Capacity,-8} {room.Type,-15} ✅ AVAILABLE ║");
                }
            }
            
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            
            // Show booked rooms too
            var allRooms = _bookingService.ConferenceRooms;
            var bookedRooms = allRooms.Except(availableRooms);
            var bookings = _bookingService.Bookings;
            
            if (bookedRooms.Any())
            {
                Console.WriteLine("\n⚠️  Booked/Unavailable Rooms:");
                foreach (var room in bookedRooms)
                {
                    // Find bookings for this room
                    var roomBookings = bookings
                        .Where(b => b.RoomId == room.Id && b.Status == BookingStatus.Confirmed)
                        .Where(b => b.StartTime < endTime && b.EndTime > startTime);
                    
                    if (roomBookings.Any())
                    {
                        var booking = roomBookings.First();
                        Console.WriteLine($"• {room.Name}: Booked by {booking.UserName} " +  // FIXED: UserName
                                        $"({booking.StartTime:HH:mm}-{booking.EndTime:HH:mm})");
                    }
                }
            }
        }
        
        static void ViewStatistics()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 SYSTEM STATISTICS                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            
            var rooms = _bookingService.ConferenceRooms;
            var bookings = _bookingService.Bookings;
            
            Console.WriteLine("\n📊 Room Statistics:");
            Console.WriteLine("──────────────────");
            Console.WriteLine($"• Total Rooms: {rooms.Count}");
            Console.WriteLine($"• Total Capacity: {rooms.Sum(r => r.Capacity)} people");
            
            var roomsByType = rooms.GroupBy(r => r.Type)
                                  .Select(g => new { Type = g.Key, Count = g.Count() });
            
            foreach (var group in roomsByType)
            {
                Console.WriteLine($"• {group.Type} Rooms: {group.Count}");
            }
            
            Console.WriteLine("\n📊 Booking Statistics:");
            Console.WriteLine("─────────────────────");
            Console.WriteLine($"• Total Bookings: {bookings.Count}");
            
            var bookingsByStatus = bookings.GroupBy(b => b.Status)
                                         .Select(g => new { Status = g.Key, Count = g.Count() });
            
            foreach (var group in bookingsByStatus)
            {
                string statusIcon = group.Status == BookingStatus.Confirmed ? "✅" :
                                  group.Status == BookingStatus.Cancelled ? "❌" :
                                  group.Status == BookingStatus.Pending ? "⏳" : "✓";
                
                Console.WriteLine($"• {statusIcon} {group.Status}: {group.Count}");
            }
            
            if (bookings.Any())
            {
                // FIXED: Group by RoomId, not booking.Room.Id
                var mostBookedRoom = bookings.GroupBy(b => b.RoomId)
                                           .Select(g => new 
                                           { 
                                               RoomId = g.Key, 
                                               Count = g.Count(),
                                               Room = rooms.FirstOrDefault(r => r.Id == g.Key)
                                           })
                                           .OrderByDescending(x => x.Count)
                                           .FirstOrDefault();
                
                if (mostBookedRoom != null && mostBookedRoom.Room != null)
                {
                    Console.WriteLine($"\n🏆 Most Popular Room: {mostBookedRoom.Room.Name} " +
                                    $"(ID: {mostBookedRoom.RoomId}, {mostBookedRoom.Count} bookings)");
                }
                
                var upcomingBookings = bookings.Count(b => b.StartTime > DateTime.Now && b.Status == BookingStatus.Confirmed);
                Console.WriteLine($"• Upcoming Confirmed Bookings: {upcomingBookings}");
                
                // Busiest time (simplified)
                var morningBookings = bookings.Count(b => b.StartTime.Hour < 12);
                var afternoonBookings = bookings.Count(b => b.StartTime.Hour >= 12 && b.StartTime.Hour < 17);
                var eveningBookings = bookings.Count(b => b.StartTime.Hour >= 17);
                
                Console.WriteLine($"\n⏰ Booking Distribution:");
                Console.WriteLine($"  • Morning (before 12:00): {morningBookings}");
                Console.WriteLine($"  • Afternoon (12:00-17:00): {afternoonBookings}");
                Console.WriteLine($"  • Evening (after 17:00): {eveningBookings}");
            }
            
            // LINQ demonstrations
            Console.WriteLine("\n🔍 LINQ Query Demonstrations:");
            Console.WriteLine("─────────────────────────────");
            
            bool hasAnyBookings = bookings.Any();
            Console.WriteLine($"• Has any bookings? {hasAnyBookings}");
            
            bool allBookingsValid = bookings.All(b => b.StartTime < b.EndTime);
            Console.WriteLine($"• All bookings have valid times? {allBookingsValid}");
            
            var firstBooking = bookings.OrderBy(b => b.StartTime).FirstOrDefault();
            if (firstBooking != null)
            {
                Console.WriteLine($"• First booking ever: ID {firstBooking.Id} on {firstBooking.StartTime:yyyy-MM-dd}");
            }
            
            // FIXED: Compare with RoomId
            var roomsWithBookings = rooms.Where(r => bookings.Any(b => b.RoomId == r.Id)).Count();
            Console.WriteLine($"• Rooms with at least one booking: {roomsWithBookings}/{rooms.Count}");
        }
    }
}
