# Conference Booking System API

## Overview

This is an ASP.NET Core Web API for managing conference room bookings.  
It is designed to demonstrate professional API architecture, separation of concerns, and API contract validation.

The project covers Assignments 2.1 through 2.2 and is prepared for Assignment 2.3.

---

## Project Structure

Conference-Booking-Domain/
├── API/ # ASP.NET Core Web API
│ ├── Controllers/ # API endpoints
│ │ ├── BookingsController.cs
│ │ ├── ConferenceRoomsController.cs
│ │ ├── FileOperationsController.cs
│ │ └── HealthController.cs
│ ├── Models/ # API DTOs
│ │ ├── BookingDto.cs
│ │ ├── CreateBookingDto.cs
│ │ ├── UpdateBookingStatusDto.cs
│ │ ├── ConferenceRoomDto.cs
│ │ └── CreateRoomDto.cs
│ ├── Program.cs
│ └── API.csproj
├── ConferenceRoomBooking.Domain/ # Core domain models and enums
├── ConferenceRoomBooking.Logic/ # Business logic (BookingManager)
└── ConferenceRoomBooking.sln

---

## Branching Strategy

- **Assignment2.1**: Initial API setup, basic controllers, BookingManager integration  
- **Assignment2.2**: Strengthened API contract  
  - DTO validation at API boundary  
  - Proper HTTP status codes (200, 400, 422, 500)  
  - Thin controllers (no domain logic in controllers)  
  - Domain failures mapped to HTTP responses  
- **Assignment2.3**: Ready for the next assignment (TBD)

---

## Features Implemented (So Far)

### Booking Operations

- Get all bookings (`GET /api/bookings`)  
- Get booking by ID (`GET /api/bookings/{id}`)  
- Create a booking (`POST /api/bookings`)  
- Update booking status (`PUT /api/bookings/{id}/status`)

### Conference Room Operations

- Get all conference rooms (`GET /api/conferencerooms`)  
- Create a new room (`POST /api/conferencerooms`)  

### Validation & Safety

- All endpoints use **DTOs**, not domain models  
- Input validation with `[Required]`, `[EmailAddress]`, `[Range]`  
- Invalid requests return `400 Bad Request`  
- Domain rule violations return `422 Unprocessable Entity`  
- Unexpected errors return `500 Internal Server Error`  
- Controllers remain thin — only map DTOs and call BookingManager

### Additional Endpoints

- Health check endpoint (`GET /api/health`)  
- File operations (`FileOperationsController`) — for CSV/JSON import/export

---

## Tools & Libraries

- .NET 8.0  
- ASP.NET Core Web API  
- Swashbuckle (Swagger) for API documentation  
- Microsoft.AspNetCore.OpenApi  
- Visual Studio / VS Code  

---

## Next Steps (Assignment 2.3)

- Implement extended API functionality  
- Additional domain rules and validation  
- Further API refinement and tests  
- Keep branching strategy consistent for each assignment

---

## How to Run

1. Clone repository:  

```bash
