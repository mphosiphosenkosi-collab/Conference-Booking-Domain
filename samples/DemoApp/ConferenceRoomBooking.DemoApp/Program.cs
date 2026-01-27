// File: samples/DemoApp/Program.cs
using System;
using ConferenceRoomBooking.Domain.Entities;
using ConferenceRoomBooking.Domain.Enums;

Console.WriteLine("=== CONFERENCE ROOM BOOKING DOMAIN MODEL DEMO ===\n");
Console.WriteLine("Assignment 1.1: Domain Modelling with C#\n");

// Create demo rooms
Console.WriteLine("📋 STEP 1: CREATING CONFERENCE ROOMS");
Console.WriteLine(new string('=', 50));

var rooms = new[]
{
    new ConferenceRoom(
        name: "Innovation Lab",
        code: "INN-01",
        roomType: RoomType.TeamRoom,
        maxCapacity: 8,
        description: "Creative space for team collaboration",
        hasProjector: true,
        hasWhiteboard: true),
    
    new ConferenceRoom(
        name: "Executive Boardroom",
        code: "EXE-01",
        roomType: RoomType.ConferenceRoom,
        maxCapacity: 20,
        description: "Premium meeting room",
        hasVideoConferencing: true,
        hasProjector: true,
        hasWhiteboard: true),
    
    new ConferenceRoom(
        name: "Quick Huddle",
        code: "QHK-01",
        roomType: RoomType.SmallMeetingRoom,
        maxCapacity: 4,
        description: "Small room for quick meetings",
        hasWhiteboard: true)
};

foreach (var room in rooms)
{
    Console.WriteLine($"• {room.Code}: {room.Name}");
    Console.WriteLine($"  Type: {room.RoomType}, Capacity: {room.MaxCapacity}");
    Console.WriteLine($"  Available: {(room.IsAvailable ? "✅" : "❌")}");
}

// Demonstrate business rules
Console.WriteLine("\n📐 STEP 2: DEMONSTRATING BUSINESS RULES");
Console.WriteLine(new string('=', 50));

Console.WriteLine("\n✓ Rule 1: Room capacity validation");
var smallRoom = rooms[2]; // Quick Huddle (capacity: 4)
try
{
    // This should fail - too many attendees
    var invalidBooking = new Booking(
        conferenceRoom: smallRoom,
        bookedBy: "Test User",
        bookerEmail: "test@company.com",
        startTime: DateTime.UtcNow.AddHours(2),
        endTime: DateTime.UtcNow.AddHours(3),
        numberOfAttendees: 10, // Exceeds capacity!
        meetingTitle: "Overcrowded Meeting");
    
    Console.WriteLine("  ✗ ERROR: Should have failed!");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  ✅ Correctly prevented: {ex.Message}");
}

Console.WriteLine("\n✓ Rule 2: Booking duration validation");
try
{
    var invalidBooking = new Booking(
        conferenceRoom: smallRoom,
        bookedBy: "Test User",
        bookerEmail: "test@company.com",
        startTime: DateTime.UtcNow.AddHours(2),
        endTime: DateTime.UtcNow.AddHours(2).AddMinutes(15), // Only 15 minutes!
        numberOfAttendees: 2,
        meetingTitle: "Too Short Meeting");
    
    Console.WriteLine("  ✗ ERROR: Should have failed!");
}
catch (ArgumentException ex) when (ex.Message.Contains("Minimum"))
{
    Console.WriteLine($"  ✅ Correctly enforced: {ex.Message}");
}

Console.WriteLine("\n✓ Rule 3: Time quarter-hour rule");
try
{
    var invalidBooking = new Booking(
        conferenceRoom: smallRoom,
        bookedBy: "Test User",
        bookerEmail: "test@company.com",
        startTime: DateTime.UtcNow.AddHours(2).AddMinutes(7), // Not on quarter hour
        endTime: DateTime.UtcNow.AddHours(3),
        numberOfAttendees: 2,
        meetingTitle: "Bad Timing");
    
    Console.WriteLine("  ✗ ERROR: Should have failed!");
}
catch (ArgumentException ex) when (ex.Message.Contains("15-minute"))
{
    Console.WriteLine($"  ✅ Correctly enforced: {ex.Message}");
}

// Demonstrate valid workflow
Console.WriteLine("\n🔄 STEP 3: VALID BOOKING WORKFLOW");
Console.WriteLine(new string('=', 50));

var boardroom = rooms[1]; // Executive Boardroom
var startTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day + 1, 14, 0, 0, DateTimeKind.Utc);
var endTime = startTime.AddHours(2);

Console.WriteLine("\n1. Creating a valid booking:");
var booking = new Booking(
    conferenceRoom: boardroom,
    bookedBy: "John Doe",
    bookerEmail: "john.doe@company.com",
    startTime: startTime,
    endTime: endTime,
    numberOfAttendees: 15,
    meetingTitle: "Quarterly Planning Meeting");

Console.WriteLine($"   ✅ Created: {booking.MeetingTitle}");
Console.WriteLine($"   Status: {booking.Status}");
Console.WriteLine($"   Room: {booking.ConferenceRoom.Name}");
Console.WriteLine($"   Time: {booking.StartTime:MMM dd, yyyy HH:mm} - {booking.EndTime:HH:mm}");

Console.WriteLine("\n2. Confirming the booking:");
booking.Confirm();
Console.WriteLine($"   ✅ Status: {booking.Status}");

Console.WriteLine("\n3. Checking if active:");
Console.WriteLine($"   Is active now? {booking.IsActive()}");
Console.WriteLine($"   Has ended? {booking.IsCompleted()}");

Console.WriteLine("\n4. Cancelling the booking:");
booking.Cancel("Meeting rescheduled");
Console.WriteLine($"   ✅ Final status: {booking.Status}");

// Show enum usage
Console.WriteLine("\n🎯 STEP 4: ENUM USAGE DEMONSTRATION");
Console.WriteLine(new string('=', 50));

Console.WriteLine("\nBookingStatus Enum Values:");
foreach (BookingStatus status in Enum.GetValues(typeof(BookingStatus)))
{
    Console.WriteLine($"  • {status} (value: {(int)status})");
}

Console.WriteLine("\nRoomType Enum Values:");
foreach (RoomType roomType in Enum.GetValues(typeof(RoomType)))
{
    Console.WriteLine($"  • {roomType} (value: {(int)roomType})");
    
    // Show business rule example
    var exampleRoom = new ConferenceRoom(
        name: $"Example {roomType}",
        code: $"EXP-{(int)roomType}",
        roomType: roomType,
        maxCapacity: roomType switch
        {
            RoomType.SmallMeetingRoom => 4,
            RoomType.TeamRoom => 10,
            RoomType.ConferenceRoom => 25,
            RoomType.Auditorium => 100,
            _ => 10
        });
    
    Console.WriteLine($"    Max capacity example: {exampleRoom.MaxCapacity}");
}

// Summary
Console.WriteLine("\n✅ ASSIGNMENT REQUIREMENTS CHECKLIST");
Console.WriteLine(new string('=', 50));

Console.WriteLine("\n✓ 1. ConferenceRoom class - IMPLEMENTED");
Console.WriteLine("   • Properties: Id, Name, Code, RoomType, MaxCapacity");
Console.WriteLine("   • Business rules: Capacity validation, equipment rules");

Console.WriteLine("\n✓ 2. Booking class - IMPLEMENTED");
Console.WriteLine("   • Properties: ConferenceRoom, BookedBy, StartTime, EndTime, Status");
Console.WriteLine("   • Business rules: Duration limits, capacity checks, time rules");

Console.WriteLine("\n✓ 3. Status enum (BookingStatus) - IMPLEMENTED");
Console.WriteLine("   • Values: Pending, Confirmed, Cancelled, Completed");
Console.WriteLine("   • Manages booking lifecycle");

Console.WriteLine("\n✓ 4. Additional enum (RoomType) - IMPLEMENTED");
Console.WriteLine("   • Values: SmallMeetingRoom, TeamRoom, ConferenceRoom, Auditorium");
Console.WriteLine("   • Business rules: Capacity ranges, booking duration limits");

Console.WriteLine("\n✓ 5. Clean, intentional domain modelling - ACHIEVED");
Console.WriteLine("   • Constructors enforce valid state");
Console.WriteLine("   • Properties with private setters for encapsulation");
Console.WriteLine("   • Business rules in domain objects (not comments)");

Console.WriteLine("\n✓ 6. .NET 8 console application - IMPLEMENTED");
Console.WriteLine("   • No external libraries used");
Console.WriteLine("   • Demonstrates all domain features");

Console.WriteLine("\n🎉 ASSIGNMENT 1.1 COMPLETE!");
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();