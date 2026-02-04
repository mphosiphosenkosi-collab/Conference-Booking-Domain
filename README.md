# 📖 Conference Room Booking System 

📋 Table of Contents
Project Overview

Architecture & Design

Project Structure

Getting Started

API Reference

Extra Credit Implementation

Testing & Verification

Development Guidelines

Interview Preparation

License

🏗️ Project Overview
Conference Room Booking System
*A Professional .NET 8 Domain-Driven Design Project*

A production-ready conference room booking system built using modern .NET 8 architecture principles. Following industry-standard Domain-Driven Design (DDD) patterns, this project demonstrates how to structure enterprise applications with clear separation of concerns, testable components, and maintainable code.

Key Features:

✅ Clean layered architecture (DDD-inspired)

✅ Repository pattern implementation

✅ Dependency injection throughout

✅ JSON file-based persistence

✅ Swagger API documentation

✅ Booking management (create, read, cancel)

✅ Overlap detection and prevention

✅ Room availability checking

✅ Status tracking (Pending, Confirmed, Cancelled)

🏛️ Architecture & Design
Layered Architecture
text
ConferenceRoomBooking/
├── 📦 Domain/          # Pure domain models
│   ├── Entities/       # Booking.cs, ConferenceRoom.cs
│   ├── Enums/          # BookingStatus.cs
│   ├── DTOs/           # BookingRequest.cs, BookingResult.cs
│   └── Exceptions/     # Domain-specific exceptions
├── ⚙️ Logic/           # Business logic
│   ├── Services/       # BookingService.cs
│   ├── Interfaces/     # IBookingService.cs
│   └── Validators/     # Business rule validation
├── 💾 Persistence/     # Data access
│   ├── Repositories/   # IBookingRepository.cs, BookingRepository.cs
│   ├── Stores/         # JsonDataStore.cs
│   └── Data/           # bookings_data.json
├── 🌐 API/             # Web interface
│   ├── Controllers/    # BookingsController.cs, HealthController.cs
│   └── Program.cs      # DI configuration
└── 💻 ConsoleApp/      # Console client application
Dependency Flow
Domain ← No dependencies (pure models)

Logic ← Domain (business logic only)

Persistence ← Domain + Logic interfaces

API ← Domain + Logic + Persistence

ConsoleApp ← Logic

Key Design Principles
Principle	Implementation	Benefit
Separation of Concerns	Each layer has single responsibility	Prevents tangled code, easier maintenance
Testability	Independent layer testing with mocking	Comprehensive automated testing
Dependency Inversion	Interfaces define contracts	Swap implementations without changing business logic
Immutability	Read-only properties in domain entities	Thread safety, predictable state
📁 Project Structure
Project	Purpose	Key Components
ConferenceRoomBooking.Domain	Core business models	Booking, ConferenceRoom, DTOs, exceptions
ConferenceRoomBooking.Logic	Business rules	BookingService, interfaces, validation
ConferenceRoomBooking.Persistence	Data storage	BookingRepository, JsonDataStore
ConferenceRoomBooking.WebApi	HTTP/REST API	Controllers, Swagger, health endpoints
ConferenceRoomBooking.ConsoleApp	CLI interface	Console-based booking operations
🚀 Getting Started
Prerequisites
.NET 8 SDK

Git

Visual Studio 2022, VS Code, or Rider

Installation
bash

# Clone repository

git clone <repository-url>
cd ConferenceRoomBooking

# Restore dependencies

dotnet restore

# Build solution

dotnet build

# Run Web API

dotnet run --project src/ConferenceRoomBooking.WebApi
Access Points
Swagger UI: https://localhost:5001/swagger

Health Check: https://localhost:5001/api/health

API Base: https://localhost:5001/api/bookings

🔌 API Reference
Core Endpoints
Method	Endpoint	Description	Status Codes
POST	/api/bookings	Create new booking	201, 400
GET	/api/bookings	Get all bookings	200
GET	/api/bookings/{id}	Get booking by ID	200, 404
DELETE	/api/bookings/{id}	Cancel booking	200, 404
GET	/api/bookings/check-availability	Check room availability	200
Extra Credit Endpoints
Method	Endpoint	Description	Status Codes
GET	/api/health	API health status	200
GET	/api/bookings/{id}	Single resource with 404	200, 404
Example Requests
bash

# Health check

curl -X GET "https://localhost:5001/api/health"

# Create booking

curl -X POST "https://localhost:5001/api/bookings" \
  -H "Content-Type: application/json" \
  -d '{
    "employeeId": "EMP001",
    "roomName": "Boardroom A",
    "startTime": "2024-12-25T09:00:00Z",
    "endTime": "2024-12-25T10:00:00Z"
  }'

# Get booking by ID

curl -X GET "https://localhost:5001/api/bookings/1"

# Get non-existent booking (returns 404)

curl -X GET "https://localhost:5001/api/bookings/999999"
🎯 Extra Credit Implementation
✅ Completed Requirements
Requirement	Status	Implementation
GET /api/bookings/{id}	✅ Done	Route parameter, returns 404 if not found
No exceptions for control flow	✅ Done	Uses NotFound() instead of throwing
GET /api/health endpoint	✅ Done	Returns service status and UTC time
RESTful thinking	✅ Done	Proper HTTP verbs and status codes
Status code discipline	✅ Done	200, 201, 400, 404 as appropriate
API maturity	✅ Done	Health endpoint for observability
Separation of concerns	✅ Done	Thin controllers, logic in services
Real-world readiness	✅ Done	Error handling, health checks, docs
Minimal API README section	✅ Done	Running instructions and examples
Health Endpoint Implementation
csharp
// In Program.cs or HealthController.cs
app.MapGet("/api/health", () => new
{
    service = "Conference Room Booking API",
    status = "Running",
    utcTime = DateTime.UtcNow
});
Response:

json
{
    "service": "Conference Room Booking API",
    "status": "Running",
    "utcTime": "2024-12-25T10:00:00Z"
}
404 Handling Implementation
csharp
[HttpGet("{id}")]
public async Task<ActionResult<BookingResponse>> GetBooking(int id)
{
    var booking = await _bookingService.GetBookingByIdAsync(id);
    
    if (booking == null)
    {
        return NotFound(); // Proper 404, no exceptions thrown
    }
    
    return Ok(booking);
}
🧪 Testing & Verification
Running Tests
bash

# Run all tests

dotnet test

# Run specific test project

dotnet test src/ConferenceRoomBooking.Logic.Tests
Verification Scripts
bash

# Check project structure

./final_structure_check.sh

# Test API behavior

./demonstrate_api_behavior.sh

# Verify dependencies (no circular references)

dotnet list src/ConferenceRoomBooking.Logic reference
Manual Testing with cURL
bash

# Test health endpoint

curl -X GET "https://localhost:5001/api/health" -k

# Test 404 response

curl -X GET "https://localhost:5001/api/bookings/999999" -k -v

# Should return: HTTP/1.1 404 Not Found

👨‍💻 Development Guidelines
Build Commands
bash
# Build all projects
dotnet build

# Run Web API
dotnet run --project src/ConferenceRoomBooking.WebApi

# Run Console App
dotnet run --project src/ConferenceRoomBooking.ConsoleApp

# Clean solution
dotnet clean
Adding New Features
Add Domain Model → Create entity in Domain/Entities/

Add Business Logic → Create service in Logic/Services/

Add Data Access → Implement repository in Persistence/Repositories/

Add API Endpoint → Create controller in API/Controllers/

Add Tests → Create test project or add to existing

Dependency Injection Registration
csharp
// In Program.cs
builder.Services.AddLogicServices();      // From Logic layer
builder.Services.AddPersistenceServices(); // From Persistence layer

// Service lifetimes:
// - Singleton: JsonDataStore (file access coordination)
// - Scoped: BookingRepository, BookingService (per-request isolation)
// - Transient: Lightweight, stateless services
💼 Interview Preparation
Common Questions & Answers
Q: Why use layered architecture?
A: Maintainability (changes in one layer don't cascade), testability (independent testing), scalability (replace layers without rewriting logic), and clear responsibility separation.

Q: How do you prevent circular dependencies?
A: Strict dependency rules: Domain (no deps), Logic (depends on Domain only), Persistence (depends on Domain + Logic interfaces), WebApi (depends on all). Interfaces are defined where they're used.

Q: DTOs vs Domain Entities?
A: DTOs are for API communication (public setters, input validation). Domain Entities are for business logic (immutable, enforce rules, have behavior). Separation prevents overposting attacks.

Q: How to add a database?
A: 1. Create SqlBookingRepository : IBookingRepository. 2. Update DI to use EF Core. 3. Add migrations. No changes needed in Domain, Logic, or WebApi layers.

Q: Why async/await everywhere?
A: Scalability (non-blocking I/O), consistency (uniform pattern), performance (better resource utilization), and future-proofing for truly async databases.

📊 Architecture Benefits Summary
Benefit	How Achieved	Impact
Maintainability	Clear layer separation	Changes isolated, reduced regression risk
Testability	Dependency injection, interfaces	Comprehensive automated testing
Scalability	Async patterns, swappable layers	Easy to add features or scale infrastructure
Professionalism	Industry-standard patterns	Production-ready, interview-ready code
Learning Value	DDD implementation	Understands enterprise architecture patterns
📄 License
MIT License - See LICENSE file for details.

🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit changes (git commit -m 'Add AmazingFeature')

Push to branch (git push origin feature/AmazingFeature)

Open a Pull Request

📞 Support & Contact
Issues: GitHub Issues

Documentation: This README and Swagger UI

Learning Path: Follow the layered architecture pattern for extensions