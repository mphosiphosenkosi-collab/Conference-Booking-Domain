# Conference Room Booking System í¿¢

A professional conference room booking system built with .NET 8, featuring clean architecture, robust business logic, and scalable design.

## í¿—ï¸ Architecture (CalculatorDomainDemo Pattern)

This project follows the exact architectural patterns from Skye's CalculatorDomainDemo:

### **Layered Architecture**
ConferenceRoomBooking/
â”œâ”€â”€ í³ Domain/ # Pure domain models (like Calculation.cs)
â”‚ â”œâ”€â”€ Entities/ # Booking.cs, ConferenceRoom.cs
â”‚ â”œâ”€â”€ Enums/ # BookingStatus.cs
â”‚ â”œâ”€â”€ DTOs/ # BookingRequest.cs, BookingResult.cs
â”‚ â””â”€â”€ Exceptions/ # Domain-specific exceptions
â”œâ”€â”€ í³ Logic/ # Business logic (like CalculationService.cs)
â”‚ â”œâ”€â”€ Services/ # BookingService.cs
â”‚ â”œâ”€â”€ Interfaces/ # IBookingService.cs
â”‚ â””â”€â”€ Validators/ # Business rule validation
â”œâ”€â”€ í³ Persistence/ # Data access (like FileCalculationStore.cs)
â”‚ â”œâ”€â”€ Repositories/ # IBookingRepository.cs, BookingRepository.cs
â”‚ â”œâ”€â”€ Stores/ # JsonDataStore.cs
â”‚ â””â”€â”€ Data/ # bookings_data.json
â”œâ”€â”€ í³ API/ # Web interface (like API/Controllers/)
â”‚ â”œâ”€â”€ Controllers/ # BookingsController.cs
â”‚ â””â”€â”€ Program.cs # DI configuration
â””â”€â”€ í³ ConsoleApp/ # Console client application

text

### **Dependency Flow**
1. **Domain** â† No dependencies (pure models)
2. **Logic** â† Domain (business logic only)
3. **Persistence** â† Domain (data access)
4. **API** â† Domain + Logic + Persistence (presentation)
5. **ConsoleApp** â† Logic (client)

## íº€ Quick Start

```bash
# Clone and navigate
git clone <repository-url>
cd ConferenceRoomBooking

# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/ConferenceRoomBooking.API
Visit https://localhost:5001/swagger for API documentation.

í³‹ Features
âœ… Core Architecture
Clean layered architecture (DDD-inspired)

Repository pattern implementation

Dependency injection throughout

JSON file-based persistence

Swagger API documentation

âœ… Booking Management
Create, read, cancel bookings

Overlap detection and prevention

Room availability checking

Status tracking (Pending, Confirmed, Cancelled)

âœ… Technical Features
.NET 8 with C# 12 features

Async/await patterns

Immutable domain models

Professional error handling

Comprehensive validation

í´§ Development
Project Structure Reference
Calculator Component	Conference Equivalent
Calculation.cs	Booking.cs
CalculationService.cs	BookingService.cs
FileCalculationStore.cs	JsonDataStore.cs
ICalculationStore.cs	IBookingRepository.cs
CalculationController.cs	BookingsController.cs
Build Commands
bash
# Build all projects
dotnet build

# Run tests
dotnet test

# Run API
dotnet run --project src/ConferenceRoomBooking.API

# Run console app
dotnet run --project src/ConferenceRoomBooking.ConsoleApp
í³š API Reference
Endpoints
POST /api/bookings - Create new booking

GET /api/bookings - Get all bookings

GET /api/bookings/{id} - Get booking by ID

DELETE /api/bookings/{id} - Cancel booking

GET /api/bookings/check-availability - Check room availability

í·ª Testing
bash
# Run all tests
dotnet test

# Test specific project
dotnet test src/ConferenceRoomBooking.Logic.Tests
í´ Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit changes (git commit -m 'Add AmazingFeature')

Push to branch (git push origin feature/AmazingFeature)

Open a Pull Request

í³„ License
MIT License - see LICENSE file for details.
