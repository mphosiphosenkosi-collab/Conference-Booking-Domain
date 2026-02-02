# Conference Room Booking System 🏢

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain)

A professional conference room booking system built with .NET 8, featuring clean architecture, robust business logic, and scalable design. This system manages room reservations, prevents scheduling conflicts, and enforces business rules through a well-structured domain model.

## 📋 Table of Contents

- [🌟 Features](#-features)
- [🏗️ Architecture](#️-architecture)
- [🚀 Quick Start](#-quick-start)
- [📁 Project Structure](#-project-structure)
- [💻 Code Examples](#-code-examples)
- [🔧 Prerequisites](#-prerequisites)
- [📚 API Reference](#-api-reference)
- [🧪 Testing](#-testing)
- [🔄 Version History](#-version-history)
- [👨‍💻 Author](#-author)
- [📄 License](#-license)

## 🌟 Features

### ✅ **Core Functionality**

- **Room Management**: Create and manage conference rooms with name, capacity, and type
- **Smart Booking**: Book rooms with automatic overlap detection
- **Status Tracking**: Real-time booking status (Pending, Confirmed, Cancelled)
- **Validation**: Comprehensive business rule enforcement
- **In-Memory Storage**: Development-ready with simple data persistence

### 🛡️ **Business Rules**

- 📅 **No Overlapping Bookings**: Prevent double-booking of rooms
- ⏰ **Valid Time Slots**: Ensure start time is before end time
- 🏢 **Room Availability**: Bookings only for existing rooms
- 🔄 **Status Transitions**: Controlled state changes only

### 🔌 **Extensible Design**

- **Clean Architecture**: Domain, Logic, and Persistence separation
- **Repository Pattern Ready**: Easy database integration
- **API Ready**: RESTful endpoints prepared
- **Testable**: Business logic isolated for easy testing

## 🏗️ Architecture

### **Layered Architecture**

┌─────────────────────────────────────────────────────┐
│ Console Application │
├─────────────────────────────────────────────────────┤
│ Business Logic Layer │
├─────────────────────────────────────────────────────┤
│ Domain Models Layer │
├─────────────────────────────────────────────────────┤
│ (Future: Persistence Layer) │
└─────────────────────────────────────────────────────┘

text

### **Technology Stack**

- **.NET 8**: Modern, high-performance framework
- **C# 12**: Latest language features
- **LINQ**: Powerful data querying
- **Collections**: Efficient data management

## 🚀 Quick Start

### **1. Clone & Setup**

```bash
# Clone the repository
git clone https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain.git

# Navigate to project
cd Conference-Booking-Domain

# Restore dependencies
dotnet restore
2. Run the Application
bash
# Run the console application
dotnet run --project src/ConferenceRoomBooking.ConsoleApp

# Or build and run separately
dotnet build
dotnet run --project src/ConferenceRoomBooking.ConsoleApp
3. Example Usage
csharp
// Create a conference room
var boardroom = new ConferenceRoom("Boardroom", 20, "Executive");

// Create a booking request
var request = new BookingRequest(
    "EMP001",
    "Boardroom",
    DateTime.Now.AddHours(1),
    DateTime.Now.AddHours(2));

// Book the room
var booking = bookingManager.CreateBooking(request, boardroom);
Console.WriteLine($"Booking #{booking.Id} created successfully!");
📁 Project Structure
text
Conference-Booking-Domain/
├── 📂 src/
│   ├── 📂 ConferenceRoomBooking.ConsoleApp/
│   │   ├── Program.cs                     # Console interface
│   │   └── ConferenceRoomBooking.ConsoleApp.csproj
│   │
│   ├── 📂 ConferenceRoomBooking.Domain/
│   │   ├── 📂 Entities/
│   │   │   ├── Booking.cs                 # Booking entity
│   │   │   └── ConferenceRoom.cs          # Room entity
│   │   ├── 📂 Enums/
│   │   │   └── BookingStatus.cs           # Status enumeration
│   │   ├── 📂 Models/
│   │   │   └── BookingRequest.cs          # Request DTO
│   │   └── ConferenceRoomBooking.Domain.csproj
│   │
│   └── 📂 ConferenceRoomBooking.Logic/
│       ├── BookingManager.cs              # Core business logic
│       ├── BookingOverlapException.cs     # Custom exception
│       └── ConferenceRoomBooking.Logic.csproj
│
├── 📂 docs/                               # Documentation
├── 📄 README.md                           # This file
├── 📄 LICENSE                             # MIT License
└── ConferenceRoomBooking.sln              # Solution file
💻 Code Examples
Creating a Conference Room
csharp
public class ConferenceRoom
{
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string RoomType { get; set; }

    public ConferenceRoom(string name, int capacity, string roomType)
    {
        Name = name;
        Capacity = capacity;
        RoomType = roomType;
    }
}

// Usage
var trainingRoom = new ConferenceRoom("Training Room A", 30, "Large");
Booking Management
csharp
public class BookingManager
{
    private readonly List<Booking> _bookings = new();
    private int _nextId = 1;

    public Booking CreateBooking(BookingRequest request, ConferenceRoom room)
    {
        // Check for overlaps
        if (HasOverlap(room.Name, request.StartTime, request.EndTime))
        {
            throw new BookingOverlapException(
                $"Room {room.Name} is already booked for the requested time.");
        }

        var booking = new Booking(
            _nextId++,
            request.EmployeeId,
            room,
            request.StartTime,
            request.EndTime,
            BookingStatus.Confirmed);

        _bookings.Add(booking);
        return booking;
    }
}
🔧 Prerequisites
.NET 8 SDK (Download)

IDE: Visual Studio 2022, VS Code, or JetBrains Rider

Git for version control

Basic C# knowledge (LINQ, OOP, Collections)

Recommended VS Code Extensions
C# for VS Code (ms-dotnettools.csharp)

.NET Core Test Explorer (formulahendry.dotnet-test-explorer)

GitLens (eamodio.gitlens)

📚 API Reference
BookingManager Class
Method	Parameters Returns Description
CreateBooking	BookingRequest request, ConferenceRoom room	Booking	Creates a new booking with validation
GetAllBookings None List<Booking>Returns all bookings
GetBookingById int id Booking	Returns booking by ID
CancelBooking	int id	void	Cancels a booking
Entities
Booking

public class Booking
{
    public int Id { get; set; }
    public string EmployeeId { get; set; }
    public ConferenceRoom Room { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
}
ConferenceRoom
csharp
public class ConferenceRoom
{
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string RoomType { get; set; }
}
🧪 Testing
Running the Console Application
bash
# Navigate to console app
cd src/ConferenceRoomBooking.ConsoleApp

# Run with sample data
dotnet run

# Expected output:
# ==================================
# Conference Room Booking System
# ==================================
# 1. Booking created: Training Room A
# 2. Overlap prevented: Boardroom
# 3. All bookings displayed...
Testing Scenarios
Valid Booking: Room available, proper time slot

Overlap Prevention: Attempt to book occupied room

Invalid Times: End time before start time

Room Management: Create and list rooms

🔄 Version History
Version 1.3.0 (Current) - Architecture Refactor
✅ New Folder Structure: Domain/Logic/Persistence architecture

✅ Logic Layer: Added BookingManager with business logic

✅ Clean Separation: Domain entities, business logic separated

✅ Updated Domain: Simplified ConferenceRoom with RoomType as string

✅ Console App: Updated to use new architecture

✅ Removed Services: Replaced with cleaner Logic layer

Version 1.2.0 - Business Logic & Collections
✅ Collections management with List<T> and Dictionary<TKey, TValue>

✅ LINQ queries for business logic

✅ Booking service implementation

✅ Business rule enforcement

Version 1.1.0 - Domain Model
✅ Initial domain model implementation

✅ Basic validation and business rules

✅ Console demonstration application

👨‍💻 Author
Siphosenkosi
GitHub: @mphosiphosenkosi-collab
Repository: Conference-Booking-Domain

About This Project
This project demonstrates professional C# development practices including:

Clean Architecture principles

Domain-Driven Design (DDD) concepts

SOLID principles implementation

Professional Git workflow

Comprehensive documentation

Connect
💼 Portfolio: GitHub Profile

📧 Contact: Available via GitHub issues or discussions

🔗 LinkedIn: Connect for professional opportunities

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

text
MIT License

Copyright (c) 2024 Siphosenkosi

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

Fork the repository

Create your feature branch (git checkout -b feature/AmazingFeature)

Commit your changes (git commit -m 'Add some AmazingFeature')

Push to the branch (git push origin feature/AmazingFeature)

Open a Pull Request

⭐ Support
If you find this project helpful, please give it a star! ⭐

<div align="center"> <sub>Built with ❤️ by <a href="https://github.com/mphosiphosenkosi-collab">Siphosenkosi</a></sub> </div>