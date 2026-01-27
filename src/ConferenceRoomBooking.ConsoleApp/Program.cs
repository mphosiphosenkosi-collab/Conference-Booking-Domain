using System;
using System.Linq;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Services.Models;
using ConferenceRoomBooking.Services.Services;

namespace ConferenceRoomBooking.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║   CONFERENCE ROOM BOOKING SYSTEM - ASSIGNMENT 1.2   ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║   Business Logic & Collections with C#              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine();
            
            // Create the booking service
            var bookingService = new BookingService();
            
            // Create sample conference rooms using the Service method
            CreateSampleRooms(bookingService);
            
            // Demonstrate business logic
            DemonstrateBusinessRules(bookingService);
            
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════════════════════");
            Console.WriteLine("All demonstrations completed successfully!");
        }
        
        static void CreateSampleRooms(BookingService bookingService)
        {
            Console.WriteLine("��� Creating sample conference rooms...");
            
            try
            {
                // Using the service method to add rooms
                bookingService.AddConferenceRoom(1, "Boardroom", 20, RoomType.Executive);
                Console.WriteLine("  ✅ Added: Boardroom (ID: 1, Capacity: 20, Type: Executive)");
                
                bookingService.AddConferenceRoom(2, "Training Room", 50, RoomType.Large);
                Console.WriteLine("  ✅ Added: Training Room (ID: 2, Capacity: 50, Type: Large)");
                
                bookingService.AddConferenceRoom(3, "Huddle Room", 6, RoomType.Standard);
                Console.WriteLine("  ✅ Added: Huddle Room (ID: 3, Capacity: 6, Type: Standard)");
                
                bookingService.AddConferenceRoom(4, "Video Conference", 12, RoomType.VideoConference);
                Console.WriteLine("  ✅ Added: Video Conference (ID: 4, Capacity: 12, Type: VideoConference)");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ❌ Error adding room: {ex.Message}");
            }
            
            Console.WriteLine($"Total rooms in system: {bookingService.ConferenceRooms.Count}");
            Console.WriteLine();
        }
        
        static void DemonstrateBusinessRules(BookingService bookingService)
        {
            Console.WriteLine("��� Demonstrating business rules with collections...");
            Console.WriteLine();
            
            // BUSINESS RULE 1: Successful booking
            Console.WriteLine("1. Testing successful booking:");
            var request1 = new BookingRequest
            {
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                UserName = "John Doe",
                Notes = "Team meeting"
            };
            
            var result1 = bookingService.CreateBooking(request1);
            Console.WriteLine($"   Request: {request1.UserName} books Room {request1.RoomId}");
            Console.WriteLine($"   Result: {(result1.IsSuccess ? "✅ SUCCESS" : "❌ FAILED")}");
            if (result1.IsSuccess)
            {
                Console.WriteLine($"   Booking ID: {result1.Booking?.Id}");
                Console.WriteLine($"   Status: {result1.Booking?.Status}");
                Console.WriteLine($"   Time: {result1.Booking?.StartTime:HH:mm} to {result1.Booking?.EndTime:HH:mm}");
            }
            Console.WriteLine();
            
            // BUSINESS RULE 2: Double-booking prevention
            Console.WriteLine("2. Testing double-booking prevention:");
            var request2 = new BookingRequest
            {
                RoomId = 1, // Same room as above
                StartTime = DateTime.Now.AddHours(1.5), // Overlapping time
                EndTime = DateTime.Now.AddHours(2.5),
                UserName = "Jane Smith"
            };
            
            var result2 = bookingService.CreateBooking(request2);
            Console.WriteLine($"   Request: {request2.UserName} books same room at overlapping time");
            Console.WriteLine($"   Result: {(result2.IsSuccess ? "✅ SUCCESS" : "❌ FAILED")}");
            if (!result2.IsSuccess)
                Console.WriteLine($"   Reason: {result2.ErrorMessage}");
            Console.WriteLine();
            
            // BUSINESS RULE 3: Booking non-existent room
            Console.WriteLine("3. Testing booking non-existent room:");
            var request3 = new BookingRequest
            {
                RoomId = 99, // Non-existent room
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now.AddHours(4),
                UserName = "Bob Johnson"
            };
            
            var result3 = bookingService.CreateBooking(request3);
            Console.WriteLine($"   Request: Book non-existent room (ID: 99)");
            Console.WriteLine($"   Result: {(result3.IsSuccess ? "✅ SUCCESS" : "❌ FAILED")}");
            if (!result3.IsSuccess)
                Console.WriteLine($"   Reason: {result3.ErrorMessage}");
            Console.WriteLine();
            
            // BUSINESS RULE 4: Valid state transitions
            Console.WriteLine("4. Testing valid state transitions:");
            if (result1.IsSuccess && result1.Booking != null)
            {
                Console.WriteLine($"   Initial state: {result1.Booking.Status}");
                
                try
                {
                    result1.Booking.Cancel();
                    Console.WriteLine($"   After Cancel(): {result1.Booking.Status} ✅");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"   Cancel failed: {ex.Message} ❌");
                }
            }
            Console.WriteLine();
            
            // Demonstrate LINQ queries
            Console.WriteLine("5. Demonstrating LINQ queries:");
            Console.WriteLine($"   Total bookings in system: {bookingService.Bookings.Count}");
            
            var bookingsForRoom1 = bookingService.GetBookingsForRoom(1);
            Console.WriteLine($"   Bookings for Room 1: {bookingsForRoom1.Count}");
            
            var availableRooms = bookingService.GetAvailableRooms(
                DateTime.Now.AddHours(5),
                DateTime.Now.AddHours(6));
            Console.WriteLine($"   Available rooms for future slot: {availableRooms.Count}");
            
            // Demonstrate Dictionary lookup
            Console.WriteLine();
            Console.WriteLine("6. Demonstrating Dictionary for fast lookups:");
            var room = bookingService.GetRoomById(2);
            if (room != null)
            {
                Console.WriteLine($"   Found room via Dictionary lookup: {room.Name}");
                Console.WriteLine($"   Capacity: {room.Capacity}, Type: {room.Type}");
            }
            
            // Demonstrate LINQ Any(), All(), FirstOrDefault()
            Console.WriteLine();
            Console.WriteLine("7. Demonstrating LINQ methods:");
            Console.WriteLine($"   Any bookings for room 1? {bookingService.HasAnyBookingsForRoom(1)}");
            Console.WriteLine($"   All rooms available at 3pm tomorrow? {bookingService.AreAllRoomsAvailable(DateTime.Now.AddDays(1).Date.AddHours(15), DateTime.Now.AddDays(1).Date.AddHours(16))}");
            
            var userBooking = bookingService.FindBookingByUserName("John Doe");
            Console.WriteLine($"   First booking for John Doe: {(userBooking != null ? $"ID {userBooking.Id}" : "Not found")}");
            
            // Demonstrate fail-fast validation
            Console.WriteLine();
            Console.WriteLine("8. Demonstrating fail-fast validation:");
            var invalidRequest = new BookingRequest
            {
                RoomId = 1,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(1), // Invalid: end before start
                UserName = "Test User"
            };
            
            var validation = bookingService.ValidateBookingRequest(invalidRequest);
            Console.WriteLine($"   Validation result: {(validation.isValid ? "✅ VALID" : "❌ INVALID")}");
            if (!validation.isValid)
                Console.WriteLine($"   Error: {validation.errorMessage}");
        }
    }
}
