# Conference Room Booking System

<div align="center">

![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=for-the-badge&logo=aspnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=entity-framework&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)
![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)

**Author:** Siphosenkosi  
**GitHub Repository:** [Conference-Booking-Domain](https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain)  
**BitCube Trainee Program Project**  
**Last Updated:** February 2026

</div>

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [Key Features](#-key-features)
- [Architecture](#️-architecture)
- [Technology Stack](#️-technology-stack)
- [Assignment Completion](#-assignment-completion)
- [Project Structure](#️-project-structure)
- [Database Schema](#-database-schema)
- [Authentication](#-authentication)
- [API Documentation](#-api-documentation)
- [Setup Instructions](#️-setup-instructions)
- [Running the Project](#-running-the-project)
- [Testing the API](#-testing-the-api)
- [Sample Requests](#-sample-requests)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Project Overview

A comprehensive, production-ready Conference Room Booking System built as part of the BitCube Trainee Program. This system demonstrates professional-grade backend development with clean architecture, robust business logic, secure authentication, and complete API functionality.

The system manages:

- 🏢 **Conference Rooms** - Create, manage, and soft-delete rooms
- 📅 **Bookings** - Create, confirm, cancel, and track bookings
- 👥 **Users & Roles** - Secure authentication with JWT and role-based access
- 📊 **Data Integrity** - Business rules, conflict detection, and audit trails

---

## ✨ Key Features

### 🔐 **Authentication & Authorization** (Assignment 2.4)

- JWT-based authentication with 1-hour token expiration
- Role-based access control with 4 distinct roles:
  - **Admin** - Full system access, conflict resolution
  - **Employee** - Create/cancel own bookings
  - **Receptionist** - Book for visitors, view all bookings
  - **Facilities Manager** - Manage rooms, soft delete/reactivate
- Secure password hashing via ASP.NET Core Identity
- Automatic user seeding with test accounts

### 🏢 **Room Management** (Assignments 3.1, 3.2, 3.4)

- Full CRUD operations with validation
- **Soft delete** implementation (preserves historical data)
- Room types: Standard, Training, Conference, Boardroom
- Location tracking and capacity management
- Active/inactive status with filtering

### 📅 **Booking Management** (Assignments 3.1, 3.2, 3.4)

- Create bookings with automatic conflict detection
- Booking status workflow: Pending → Confirmed → Completed
- **Cancellation with audit trail** (CancelledAt timestamp)
- **Double-booking prevention** enforced at domain level
- CreatedAt timestamp for audit purposes

### 🚀 **API Features** (Assignment 3.3)

- **Pagination** with page, pageSize, totalCount metadata
- **Filtering** by room, location, date range
- **Sorting** by date, room, creation time
- **DTO projection** for efficient data transfer
- **AsNoTracking()** for read-only query optimization

### 🛡️ **Error Handling** (Assignment 2.3)

- Centralized exception handling middleware
- Domain-specific exceptions:
  - `BookingConflictException` (409 Conflict)
  - `NoBookingsException` (404 Not Found)
  - `NoConferenceRoomsException` (404 Not Found)
- Consistent JSON error response format
- HTTP status code mapping

### 🗄️ **Data Persistence** (Assignments 3.1, 3.2)

- Entity Framework Core with SQLite
- Code-first migrations with schema evolution
- Seed data for development and testing
- Foreign key relationships with cascade delete

---

## 🏗️ Architecture

The project follows **Clean Architecture** principles with clear separation of concerns:
┌─────────────────────────────────────────────────────────────────────┐
│ PRESENTATION LAYER │
│ API (Controllers) │
│ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ │
│ │ Auth │ │ Room │ │ Booking │ │ Admin │ │
│ │ Controller │ │ Controller │ │ Controller │ │ Controller │ │
│ └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│ MIDDLEWARE │
│ ┌───────────────────────────────────────────────────────────────┐ │
│ │ ExceptionHandlingMiddleware - Global error handling │ │
│ └───────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│ BUSINESS LOGIC LAYER │
│ Domain/Logic/ │
│ ┌───────────────────────────────────────────────────────────────┐ │
│ │ BookingManager - Booking business rules │ │
│ │ RoomManager - Room business rules │ │
│ └───────────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│ DOMAIN LAYER │
│ Domain/Entities/ │
│ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐ │
│ │ Booking │ │ ConferenceRoom│ │ Enums │ │ Exceptions │ │
│ └──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘ │
├─────────────────────────────────────────────────────────────────────┤
│ INFRASTRUCTURE LAYER │
│ Domain/Persistence/ │
│ ┌───────────────────────────────────────────────────────────────┐ │
│ │ IBookingStore (Interface) │ │
│ │ EFBookingStore - Database implementation │ │
│ │ BookingFileStore - File-based storage (legacy) │ │
│ └───────────────────────────────────────────────────────────────┘ │
│ API/Data/ │
│ ┌───────────────────────────────────────────────────────────────┐ │
│ │ AppDbContext - Entity Framework Core │ │
│ │ Migrations/ - Database schema versioning │ │
│ └───────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────────┘

text

---

## 🛠️ Technology Stack

### Backend Framework

- **.NET 8** - Modern, cross-platform framework
- **C# 12** - Latest language features
- **ASP.NET Core Web API** - RESTful API development

### Data Access

- **Entity Framework Core 8** - ORM for database operations
- **SQLite** - Lightweight, file-based database
- **Code First Migrations** - Schema version control

### Authentication

- **ASP.NET Core Identity** - User management
- **JWT Bearer Authentication** - Stateless authentication
- **Role-based Authorization** - Fine-grained access control

### API Documentation

- **Swagger/OpenAPI** - Interactive API documentation
- **Swashbuckle** - Swagger integration

### Development Tools

- **Git** - Version control
- **GitHub Actions** - CI/CD workflows
- **Postman** - API testing (recommended)

---

## ✅ Assignment Completion

### 📌 Assignment 2.3 - Exception Handling

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| Centralized exception handling | `ExceptionHandlingMiddleware` | ✅ Complete |
| Consistent error response format | JSON with error/detail fields | ✅ Complete |
| Domain-specific exceptions | BookingConflictException, NoBookingsException, NoConferenceRoomsException | ✅ Complete |
| HTTP status code mapping | 409 Conflict, 404 Not Found, 500 Internal Error | ✅ Complete |
| Logging | Integrated with ILogger | ✅ Complete |

### 📌 Assignment 2.4 - Authentication & Authorization

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| Identity integration | `ApplicationUser` extends `IdentityUser` | ✅ Complete |
| JWT token generation | `TokenService` with 1-hour expiry | ✅ Complete |
| Role-based authorization | 4 roles with distinct permissions | ✅ Complete |
| User seeding | `IdentitySeeder` with test accounts | ✅ Complete |
| Login endpoint | `POST /api/auth/login` | ✅ Complete |
| Secure endpoints | `[Authorize]` and `[Authorize(Roles)]` attributes | ✅ Complete |

### 📌 Assignment 3.1 - Persistence

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| EF Core configuration | `AppDbContext` with SQLite | ✅ Complete |
| Database migrations | InitialIdentity, InitialCreate | ✅ Complete |
| Repository pattern | `IBookingStore` interface | ✅ Complete |
| Async operations | All database calls use async/await | ✅ Complete |
| Data persistence | Data survives application restarts | ✅ Complete |

### 📌 Assignment 3.2 - Schema Evolution

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| Room enhancements | Location, IsActive fields | ✅ Complete |
| Booking enhancements | Status, CreatedAt, CancelledAt | ✅ Complete |
| Session entity | Capacity, StartTime, EndTime | ✅ Complete |
| Seed data | Active/inactive rooms, sessions, non-default bookings | ✅ Complete |
| Migration evidence | Migration files with documentation | ✅ Complete |

### 📌 Assignment 3.3 - Querying & Performance

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| Filtering | By room, location, date range | ✅ Complete |
| Pagination | Page, pageSize, totalCount metadata | ✅ Complete |
| Sorting | By date, room, creation time | ✅ Complete |
| DTO projection | `BookingListItemDto`, `RoomListItemDto` | ✅ Complete |
| AsNoTracking() | Used for read-only queries | ✅ Complete |
| Database-level filtering | All filters applied in SQL | ✅ Complete |

### 📌 Assignment 3.4 - Data Integrity

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| Foreign key relationships | Bookings.RoomId → Rooms.ID | ✅ Complete |
| Soft delete | Rooms.IsActive flag | ✅ Complete |
| Business rules in service layer | Double-booking prevention, active room checks | ✅ Complete |
| Referential integrity | Cascade delete configured | ✅ Complete |
| Reactivation endpoint | `POST /api/rooms/{id}/reactivate` | ✅ Complete |

---

## 📁 Project Structure

Conference-Booking-Domain/
│
├── ConferenceRoomBooking.sln # Main solution file
│
├── API/ # Presentation Layer
│ ├── Auth/ # Authentication services
│ │ ├── ApplicationUser.cs # Custom Identity user
│ │ ├── IdentitySeeder.cs # User/role seeding
│ │ └── TokenService.cs # JWT generation
│ │
│ ├── Controllers/ # API endpoints
│ │ ├── AuthController.cs # Login endpoint
│ │ ├── RoomController.cs # Room management
│ │ ├── BookingController.cs # Booking management
│ │ └── AdminController.cs # Admin functions
│ │
│ ├── Data/ # Database context
│ │ ├── AppDbContext.cs # EF Core context
│ │ └── Migrations/ # Database migrations
│ │
│ ├── DTOs/ # Data Transfer Objects
│ │ ├── BookingListItemDto.cs
│ │ ├── RoomListItemDto.cs
│ │ └── authorizeLoginDto.cs
│ │
│ ├── Middleware/ # Custom middleware
│ │ └── ExceptionHandlingMiddleware.cs # Global error handler
│ │
│ ├── Persistence/ # Repository implementations
│ │ ├── EFBookingStore.cs # Database store
│ │ └── BookingFileStore.cs # File-based store (legacy)
│ │
│ ├── Properties/ # Launch settings
│ ├── appsettings.json # Configuration
│ ├── Program.cs # Application entry point
│ └── API.csproj # Project file
│
├── Domain/ # Domain Layer
│ ├── Entities/ # Core business entities
│ │ ├── Booking.cs
│ │ └── ConferenceRoom.cs
│ │
│ ├── Enums/ # Enumerations
│ │ └── Enums.cs
│ │
│ ├── Exceptions/ # Domain exceptions
│ │ ├── BookingConflictException.cs
│ │ ├── NoBookingsException.cs
│ │ └── NoConferenceRoomsException.cs
│ │
│ ├── DTOs/ # Request/Response objects
│ │ ├── BookingRequest.cs
│ │ └── RoomRequest.cs
│ │
│ ├── Interfaces/ # Repository interfaces
│ │ └── IBookingStore.cs
│ │
│ ├── Logic/ # Business logic
│ │ ├── BookingManager.cs
│ │ └── RoomManager.cs
│ │
│ └── Domain.csproj # Project file
│
├── bookings.json # Sample booking data
├── LICENSE # MIT License
└── README.md # This file

text

---

## 📊 Database Schema

The database consists of **9 tables** with proper relationships:

### Identity Tables (7)

```sql
-- User accounts
AspNetUsers (
    Id TEXT PRIMARY KEY,
    UserName TEXT,
    Email TEXT,
    PasswordHash TEXT,  -- NEVER plain text!
    -- ... additional columns
)

-- Role definitions
AspNetRoles (
    Id TEXT PRIMARY KEY,
    Name TEXT  -- 'Admin', 'Employee', 'Receptionist', 'Facilities Manager'
)

-- User-Role assignments (many-to-many)
AspNetUserRoles (
    UserId TEXT REFERENCES AspNetUsers(Id),
    RoleId TEXT REFERENCES AspNetRoles(Id),
    PRIMARY KEY (UserId, RoleId)
)

-- Additional Identity tables: AspNetRoleClaims, AspNetUserClaims, 
-- AspNetUserLogins, AspNetUserTokens
Domain Tables (2)
sql
-- Conference rooms
conRooms (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    RoomNumber TEXT NOT NULL,
    Capacity INTEGER NOT NULL,
    type INTEGER NOT NULL,        -- 0=Standard, 1=Training, 2=Conference, 3=Boardroom
    location TEXT NOT NULL,
    IsActive INTEGER NOT NULL      -- 1=active, 0=soft-deleted
)

-- Bookings
bookings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RoomID INTEGER NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL,
    Status INTEGER NOT NULL,       -- 0=Pending, 1=Confirmed, 2=Cancelled, 3=Completed
    CreatedAt TEXT NOT NULL,
    CancelledAt TEXT NULL,
    FOREIGN KEY (RoomID) REFERENCES conRooms(ID) ON DELETE CASCADE
)

-- Index for performance
CREATE INDEX IX_bookings_RoomID ON bookings(RoomID);
Entity Relationships
One-to-Many: One Room → Many Bookings

Foreign Key: bookings.RoomID references conRooms.ID

Cascade Delete: Deleting a room deletes all its bookings

🔐 Authentication
Default Users (Seeded Automatically)
Username Password Role Permissions
Siphosenkosi siphosenkosi123 Admin Full system access
employee1 Employee123! Employee Create/cancel own bookings
employee2 Employee123! Employee Create/cancel own bookings
reception1 Reception123! Receptionist Book for visitors, view all
facilities1 Facilities123! Facilities Manager Manage rooms
Login Endpoint
Request:

http
POST /api/auth/login
Content-Type: application/json

{
    "username": "Siphosenkosi",
    "password": "siphosenkosi123"
}
Response:

json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "Siphosenkosi",
    "roles": ["Admin"]
}
Using the Token
http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
📚 API Documentation
Base URL
text
Development: https://localhost:5001
Production:  http://localhost:5000
Swagger UI:  https://localhost:5001/swagger
Authentication Endpoints
Method Endpoint Description Access
POST /api/auth/login Authenticate user Public
Room Endpoints
Method Endpoint Description Access
GET /api/rooms Get all rooms (paginated) Authenticated
GET /api/rooms/{id} Get room by ID Authenticated
POST /api/rooms Create new room Facilities Manager
PATCH /api/rooms Soft delete room Facilities Manager, Admin
POST /api/rooms/{id}/reactivate Reactivate room Facilities Manager, Admin
Booking Endpoints
Method Endpoint Description Access
GET /api/bookings Get filtered bookings Authenticated
GET /api/bookings/all Get all bookings Admin only
POST /api/bookings Create booking Employee, Receptionist
DELETE /api/bookings Cancel booking Employee (own), Receptionist
Admin Endpoints
Method Endpoint Description Access
GET /api/admin/bookings/conflicts Get conflicting bookings Admin
POST /api/admin/bookings/resolve-conflict Resolve conflict Admin
Query Parameters (Assignment 3.3)
text
GET /api/bookings?page=2&pageSize=10&roomId=5&location=Floor%201&fromDate=2026-03-01&toDate=2026-03-31&sortBy=date
Parameter Type Description Default
page int Page number 1
pageSize int Items per page 10
roomId int Filter by room ID null
location string Filter by location null
fromDate datetime Filter by start date null
toDate datetime Filter by end date null
sortBy string Sort field (date/room/created) "date"
sortOrder string Sort order (asc/desc) "asc"
⚙️ Setup Instructions
Prerequisites
.NET 8 SDK

Git

Visual Studio 2022 or VS Code

Postman (for API testing)

Installation Steps
bash
# 1. Clone the repository
git clone https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain.git
cd Conference-Booking-Domain

# 2. Restore dependencies
dotnet restore

# 3. Update database (creates SQLite file)
cd API
dotnet ef database update

# 4. Build the solution
dotnet build

# 5. Run the API
dotnet run
Configuration (appsettings.json)
json
{
  "ConnectionStrings": {
    "BookingDb": "Data Source=BookingDb.db"
  },
  "Jwt": {
    "Key": "your-super-secret-key-that-is-at-least-32-characters-long",
    "Issuer": "BookingSystemAPI",
    "Audience": "BookingSystemClient"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
🚀 Running the Project
Development Mode
bash
# Navigate to API project
cd API

# Run with hot reload
dotnet watch run

# The API will be available at:
# - http://localhost:5000
# - https://localhost:5001
# - Swagger UI: https://localhost:5001/swagger
Production Build
bash
# Publish the API
cd API
dotnet publish -c Release -o ./publish

# Run published version
cd publish
dotnet API.dll
🧪 Testing the API
Using Swagger UI
Run the API: dotnet run

Open browser: https://localhost:5001/swagger

Try endpoints interactively

Using curl
bash
# 1. Login as Admin
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"Siphosenkosi","password":"siphosenkosi123"}'

# Save the token
export TOKEN="eyJhbGciOiJIUzI1NiIs..."

# 2. Get all rooms (paginated)
curl -H "Authorization: Bearer $TOKEN" \
  "https://localhost:5001/api/rooms?page=1&pageSize=5"

# 3. Create a booking
curl -X POST -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"roomId":1,"startTime":"2026-03-01T09:00:00Z","endTime":"2026-03-01T10:00:00Z"}' \
  https://localhost:5001/api/bookings

# 4. Get filtered bookings
curl -H "Authorization: Bearer $TOKEN" \
  "https://localhost:5001/api/bookings?roomId=1&fromDate=2026-03-01&page=1&pageSize=10"
Using Postman
Import the provided Postman collection (if available)

Set up environment variables:

base_url: https://localhost:5001

token: (will be set after login)

Test endpoints sequentially

📝 Sample Requests
Create Room (Facilities Manager only)
http
POST /api/rooms
Authorization: Bearer <token>
Content-Type: application/json

{
    "roomNumber": "C303",
    "capacity": 25,
    "type": 2,
    "location": "Floor 3, Building C"
}
Create Booking (Employee)
http
POST /api/bookings
Authorization: Bearer <token>
Content-Type: application/json

{
    "roomId": 1,
    "startTime": "2026-03-15T09:00:00Z",
    "endTime": "2026-03-15T11:00:00Z"
}
Cancel Booking
http
DELETE /api/bookings
Authorization: Bearer <token>
Content-Type: application/json

{
    "bookingId": 5
}
Admin - Resolve Conflict
http
POST /api/admin/bookings/resolve-conflict
Authorization: Bearer <token>
Content-Type: application/json

{
    "bookingId": "42",
    "resolution": "approve",
    "notes": "VIP client request"
}

### 📌 Assignment 1.1 - Frontend: Component Architecture & Static UI

| Requirement | Implementation | Status |
|-------------|---------------|--------|
| **Project Initialization** | Vite + React, boilerplate removed | ✅ Complete |
| **Component Decomposition** | NavBar, Footer, Button, BookingCard, BookingList components | ✅ Complete |
| **Props & Reusability** | All data passed via props, no hardcoded text | ✅ Complete |
| **List Rendering** | mockData.js with 6 bookings, .map(), unique keys | ✅ Complete |
| **Styling & Layout** | Component-specific CSS files, responsive grid | ✅ Complete |

#### 🗂️ Frontend Project Structure
conference-booking-frontend/
├── src/
│ ├── components/
│ │ ├── NavBar/ # Navigation bar component
│ │ │ ├── NavBar.jsx
│ │ │ └── NavBar.css
│ │ ├── Footer/ # Footer component
│ │ │ ├── Footer.jsx
│ │ │ └── Footer.css
│ │ ├── Button/ # Reusable button component
│ │ │ ├── Button.jsx
│ │ │ └── Button.css
│ │ └── BookingCard/ # Booking display components
│ │ ├── BookingCard.jsx # Individual booking card
│ │ ├── BookingCard.css
│ │ ├── BookingList.jsx # Handles list of bookings
│ │ └── BookingList.css
│ ├── data/
│ │ └── mockData.js # 6 mock bookings for testing
│ ├── App.jsx # Main app assembly
│ └── App.css # Global styles
├── index.html
├── package.json
└── vite.config.js

text

#### 🧩 Component Architecture

Each component is **self-contained** in its own folder with:
- ✅ Component logic (`.jsx`)
- ✅ Component-specific styles (`.css`)
- ✅ Easy to maintain and modify
- ✅ No style conflicts between components

#### 🎨 Component Features

| Component | Features | Styles |
|-----------|----------|--------|
| **NavBar** | Logo, navigation links, user placeholder | Dark theme (#2c3e50), blue accent (#3498db), hover effects |
| **Footer** | Dynamic copyright year, React badge | Matching navbar colors, sticky positioning |
| **Button** | 3 variants (primary, secondary, danger), 3 sizes | Color-coded, hover effects, disabled state |
| **BookingCard** | Displays booking details, status badges | Hover lift effect, status colors, uses Button component |
| **BookingList** | Manages grid layout, handles edit/cancel actions | Responsive grid with auto-fill |

#### 📊 Mock Data Sample (6 Bookings)

```javascript
const mockBookings = [
  { id: 1, roomName: 'Conference Room A', date: '2024-05-20', startTime: '10:00 AM', endTime: '12:00 PM', userName: 'John Smith', status: 'confirmed' },
  { id: 2, roomName: 'Meeting Room B', date: '2024-05-20', startTime: '2:00 PM', endTime: '4:00 PM', userName: 'Sarah Johnson', status: 'pending' },
  { id: 3, roomName: 'Board Room', date: '2024-05-21', startTime: '9:00 AM', endTime: '11:00 AM', userName: 'Mike Wilson', status: 'confirmed' },
  { id: 4, roomName: 'Training Room', date: '2024-05-21', startTime: '1:00 PM', endTime: '3:00 PM', userName: 'Emily Davis', status: 'confirmed' },
  { id: 5, roomName: 'Conference Room A', date: '2024-05-22', startTime: '3:00 PM', endTime: '5:00 PM', userName: 'Tom Brown', status: 'cancelled' },
]
🎯 Status Color Coding
Status	Color	Icon
Confirmed	Green (#27ae60)	✅
Pending	Orange (#f39c12)	⏳
Cancelled	Red (#e74c3c)	❌
🚀 Running the Frontend
bash
# Navigate to frontend directory
cd conference-booking-frontend

# Install dependencies
npm install

# Start development server
npm run dev

# Open in browser
# http://localhost:5173
📱 Responsive Design
Desktop: 3-4 cards per row

Tablet: 2 cards per row

Mobile: 1 card per row

Footer: Always sticks to bottom

✨ Interactive Features
Hover effects: Cards lift up, buttons change color

Click handlers: Edit/Cancel buttons show alerts

Status badges: Color-coded for quick recognition

Smooth transitions: All animations are fluid

✅ Assignment 1.1 Completion Checklist
Vite project initialized and cleaned

Component decomposition (5 components)

Each component in its own folder with CSS

Props used for all dynamic data

Mock data with 6 bookings

List rendering with .map() and unique keys

Responsive grid layout

Professional styling with hover effects

No API calls (using mock data)

Functional components only

Clean App.jsx (only orchestrates components)

📸 Screenshots
(Add screenshots of your running application here)

Main view: All 6 booking cards in grid

Hover effect: Card lifting on hover

Mobile view: Responsive stacking

🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit your changes (git commit -m 'Add AmazingFeature')

Push to the branch (git push origin feature/AmazingFeature)

Open a Pull Request

Coding Standards
Follow C# coding conventions

Add XML comments for public APIs

Write unit tests for new features

Update documentation as needed

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

👏 Acknowledgments
BitCube for the trainee program opportunity

Mentors for guidance and code reviews

Peers for collaboration and feedback

.NET Community for excellent documentation and tools

📬 Contact
Author: Siphosenkosi
GitHub: @mphosiphosenkosi-collab
Repository: Conference-Booking-Domain

<div align="center">
⭐ Star this repository if you find it useful! ⭐

Built with ❤️ by Siphosenkosi
BitCube Trainee Program Project
Last Updated: February 2026

</div> ```
