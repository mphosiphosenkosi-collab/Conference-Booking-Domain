# Conference Room Booking System

## 📋 Project Overview

A clean, intentional C# domain model representing the core business concepts of a Conference Room Booking System. This model enforces business rules, prevents invalid states, and is designed for extension into Web APIs.

## 🎯 Assignment 1.1 - Domain Model (Completed)

- **ConferenceRoom** class representing physical rooms
- **Booking** class managing reservations
- **BookingStatus** enum (Pending, Confirmed, Cancelled, Completed)
- **RoomType** enum (Standard, Large, Executive, VideoConference)
- Domain logic encapsulated within objects
- Validation preventing invalid states

## 🎯 Assignment 1.2 - Business Logic & Collections (Current)

**Objective:** Extend the system from static domain modelling to working business logic using C# collections and LINQ.

### ✅ Implemented Features

- **Collections Management:**
  - `List<ConferenceRoom>` for all conference rooms
  - `List<Booking>` for all bookings
  - `Dictionary<TKey, TValue>` for efficient lookups
  
- **Business Rules Enforcement:**
  - No double-booking for overlapping time slots
  - Booking must reference an existing conference room
  - Valid booking state transitions only
  - Early rejection of invalid requests (fail-fast)

- **LINQ Usage:**
  - Filter collections using `Where()`
  - Check conditions using `Any()` and `All()`
  - Find items using `FirstOrDefault()`

### 🏗️ Project Structure

Conference-Booking-Domain/
├── ConferenceRoomBooking.sln
├── src/
│ ├── ConferenceRoomBooking.Domain/ # Domain models from Assignment 1.1
│ │ ├── Entities/
│ │ │ ├── Booking.cs
│ │ │ └── ConferenceRoom.cs
│ │ ├── Enums/
│ │ │ ├── BookingStatus.cs
│ │ │ └── RoomType.cs
│ │ └── ConferenceRoomBooking.Domain.csproj
│ ├── ConferenceRoomBooking.Services/ # Business logic layer (NEW)
│ │ ├── BookingService.cs
│ │ └── ConferenceRoomBooking.Services.csproj
│ └── ConferenceRoomBooking.ConsoleApp/ # Demo application
│ ├── Program.cs
│ └── ConferenceRoomBooking.ConsoleApp.csproj
└── samples/ # Additional examples

text

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain.git
cd Conference-Booking-Domain
2. Switch to Assignment 1.2 branch:
bash
git checkout Assignment-1.2
3. Restore dependencies:
bash
dotnet restore
4. Run the console application:
bash
cd src/ConferenceRoomBooking.ConsoleApp
dotnet run
🧪 Testing the System
The console application demonstrates:

Room creation with validation

Booking creation with business rules

Collection-based operations

LINQ queries for business logic

Business rule enforcement

Invalid operation prevention

🎯 Design Decisions
Separation of Concerns: Domain models, business logic, and UI are separate layers

Collections: Intentional use of List<T> and Dictionary<TKey, TValue> based on requirements

LINQ: Used for clean, readable business logic queries

Fail-fast: Invalid requests are rejected immediately

Encapsulation: Business rules are encapsulated in service layer

🔧 Prerequisites
.NET 8.0 SDK or later

Visual Studio 2022, VS Code, or any C# IDE

Git for version control

📝 Git Workflow
This assignment is developed on the Assignment-1.2 branch to practice:

Branch-based development

Merge conflict resolution

Feature isolation

Clean commit history

👥 Author
Siphosenkosi - GitHub Profile

Repository: https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain

🔄 Version History
1.1.0 (Previous)
Initial domain model implementation

Basic validation and business rules

Console demonstration application

1.2.0 (Current)
Business logic layer implementation

Collections management with C#

LINQ queries for business rules

Enhanced console application

Project structure reorganization

📄 License

MIT License - see LICENSE file for details
