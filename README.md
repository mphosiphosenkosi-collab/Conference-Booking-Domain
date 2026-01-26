# Conference Room Booking System - Domain Model

## 📋 Project Overview
A clean, intentional C# domain model representing the core business concepts of a Conference Room Booking System. This model enforces business rules, prevents invalid states, and is designed for extension into Web APIs.

## 🎯 Assignment Requirements
- **ConferenceRoom** class representing physical rooms
- **Booking** class managing reservations
- **BookingStatus** enum (Pending, Confirmed, Cancelled, Completed)
- **RoomType** enum (Standard, Large, Executive, VideoConference)
- Domain logic encapsulated within objects
- Validation preventing invalid states

## 🏗️ Domain Model Structure
ConferenceRoomBooking.Domain/
├── ConferenceRoom.cs # Room entity with capacity, type, availability
├── Booking.cs # Reservation with dates, status, business rules
├── BookingStatus.cs # Enum: Pending, Confirmed, Cancelled, Completed
└── RoomType.cs # Enum: Standard, Large, Executive, VideoConference

## 🚀 Getting Started
1. **Clone the repository:**
   ```bash
   git clone https://github.com/YOUR-USERNAME/conference-room-booking-system.git
   cd conference-room-booking-system

  Restore dependencies:

bash
dotnet restore
Run the console application:

bash
dotnet run

🧪 Testing the Domain Model
The solution includes a console application demonstrating:

Room creation with validation

Booking creation with business rules

Status transitions (Pending → Confirmed → Completed/Cancelled)

Invalid operation prevention

🎯 Design Decisions
Classes over records: Used class for mutability and encapsulation of behavior

Immutable IDs: Id properties have private setters to prevent modification

Rich domain model: Business logic (confirm, cancel) lives in domain objects

Fail-fast validation: Constructors validate input immediately

Intentional naming: Properties and methods reflect business language

📁 Project Structure/
├── src/
│   └── ConferenceRoomBooking.Domain/  # Core domain model
│       ├── ConferenceRoom.cs
│       ├── Booking.cs
│       ├── BookingStatus.cs
│       └── RoomType.cs
├── tests/                              # (Future) Unit tests
├── samples/                            # (Future) Usage examples
├── README.md                           # This file
└── ConferenceRoomBooking.sln           # Solution file

🔧 Prerequisites
.NET 

Visual Studio 2022, VS Code, or any C# IDE

📝 License
MIT License - see LICENSE file for details

👥 Author
[Siphosenkosi] - [https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain]

🔄 Version History
1.0.0 (Current): Initial domain model implementation

  
