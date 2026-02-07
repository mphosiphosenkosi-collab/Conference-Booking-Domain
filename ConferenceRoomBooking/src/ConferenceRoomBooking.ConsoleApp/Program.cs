using ConferenceRoomBooking.Domain.DTOs;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;
using ConferenceRoomBooking.Logic;        // For AddLogicServices()
using ConferenceRoomBooking.Persistence;  // For AddPersistenceServices()
using Microsoft.Extensions.DependencyInjection;

namespace ConferenceRoomBooking.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Conference Room Booking System");
        Console.WriteLine("===============================\n");
        
        // Get the data file path
        var dataFilePath = GetDataFilePath();
        Console.WriteLine($"Data file: {dataFilePath}\n");
        
        // Setup dependency injection
        var serviceProvider = new ServiceCollection()
            .AddLogicServices()           // Now this will be found!
            .AddPersistenceServices(dataFilePath)
            .BuildServiceProvider();
        
        var bookingService = serviceProvider.GetRequiredService<ConferenceRoomBooking.Logic.Interfaces.IBookingService>();
        
        // Demo: Create a booking
        Console.WriteLine("1. Creating a booking...");
        
        var request = new BookingRequest
        {
            EmployeeId = "EMP001",
            RoomName = "Boardroom",
            StartTime = DateTime.Now.AddHours(1),
            EndTime = DateTime.Now.AddHours(2)
        };
        
        try
        {
            var result = await bookingService.CreateBookingAsync(request);
            
            if (result.Success)
            {
                Console.WriteLine($"✅ Booking created successfully!");
                Console.WriteLine($"   Booking ID: {result.Booking?.Id}");
                Console.WriteLine($"   Employee: {result.Booking?.EmployeeId}");
                Console.WriteLine($"   Room: {result.Booking?.RoomId}");
                Console.WriteLine($"   Time: {result.Booking?.StartTime} to {result.Booking?.EndTime}");
            }
            else
            {
                Console.WriteLine($"❌ Booking failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
        
        // Demo: Get all bookings
        Console.WriteLine("\n2. Listing all bookings...");
        
        var bookings = await bookingService.GetAllBookingsAsync();
        
        if (bookings.Any())
        {
            foreach (var booking in bookings)
            {
                Console.WriteLine($"   • Booking #{booking.Id}: {booking.EmployeeId} in Room {booking.RoomId}");
            }
        }
        else
        {
            Console.WriteLine("   No bookings found.");
        }
        
        Console.WriteLine("\n=== DEMO COMPLETE ===");
    }
    
    private static string GetDataFilePath()
    {
        // For ConsoleApp, the path is relative to where it runs
        var projectDir = Directory.GetCurrentDirectory();
        var solutionDir = Directory.GetParent(projectDir)?.Parent?.FullName;
        
        if (solutionDir != null)
        {
            return Path.Combine(solutionDir, "src", "ConferenceRoomBooking.Persistence", "Data", "bookings_data.json");
        }
        
        // Fallback path
        return Path.Combine("..", "..", "src", "ConferenceRoomBooking.Persistence", "Data", "bookings_data.json");
    }
}