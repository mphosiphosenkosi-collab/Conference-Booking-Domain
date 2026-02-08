# Conference Room Booking System

**Author:** Siphosenkosi  
**GitHub Repository:** [Conference-Booking-Domain](https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain)

**License:** MIT  
**BitCube Trainee Program Project**

## 📋 Table of Contents

1. [Project Overview](#-project-overview)
2. [Project Structure](#️-project-structure)
3. [Technology Stack](#️-technology-stack)
4. [Assignment Progress](#-assignment-progress)
5. [Setup Instructions](#️-setup-instructions)

6. [API Documentation](#-api-documentation)
7. [Running the Project](#-running-the-project)
8. [Testing](#-testing)
9. [Contributing](#-contributing)
10. [License](#-license)

## ��� Project Overview

A comprehensive Conference Room Booking System built as part of the BitCube Trainee Program. This system manages room availability, booking requests, and booking status through a robust Web API with proper domain modeling and business logic.

## ���️ Project Structure

Conference-Booking-Domain/
├── ConferenceRoomBooking.sln # Main solution file
├── ConferenceRoomBooking/ # Core domain and logic
│ ├── ConferenceRoomBooking.Domain/ # Domain models, enums, exceptions
│ └── ConferenceRoomBooking.Logic/ # Business logic and services
├── API/ # Web API project (Assignment 2.1+)
│ ├── Controllers/ # API controllers
│ ├── Models/ # DTOs and view models
│ ├── Middleware/ # Custom middleware
│ ├── Program.cs # API entry point
│ └── API.csproj # API project file
├── .github/ # GitHub workflows
├── bookings.json # Sample data file
├── LICENSE # MIT License
└── README.md # This file

## ���️ Technology Stack

- **.NET 8** - Backend framework
- **C# 12** - Programming language
- **ASP.NET Core Web API** - REST API framework
- **Swagger/OpenAPI** - API documentation
- **Git** - Version control
- **JSON** - Data serialization

## ��� Assignment Progress

### ✅ Completed Assignments

- **Assignment 1.1**: Domain Modelling with C#
- **Assignment 1.2**: Business Logic & Collections
- **Assignment 1.3**: Robustness, Failures & Async Operations
- **Assignment 2.1**: Web API Foundation

### ��� In Progress

- **Assignment 2.4**: Advanced API Features

## ⚙️ Setup Instructions

### Prerequisites

- .NET 8 SDK
- Git
- Code editor (VS Code, Visual Studio, or Rider)

### Installation

```bash

# Clone the repository
git clone https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain.git
cd Conference-Booking-Domain

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
��� API Documentation
Base URL
text
http://localhost:5000
https://localhost:5001
Available Endpoints
Health Check
http
GET /api/health
Response:

json
{
  "status": "healthy",
  "timestamp": "2024-02-07T23:30:45Z",
  "service": "Conference Room Booking API",
  "version": "2.1.0"
}
Conference Rooms
http
GET /api/conferencerooms
Get all conference rooms

http
GET /api/conferencerooms/{id}
Get specific room by ID

http
POST /api/conferencerooms
Create new conference room

Bookings
http
GET /api/bookings
Get all bookings

http
GET /api/bookings/{id}
Get specific booking by ID

http
POST /api/bookings
Create new booking

http
PUT /api/bookings/{id}/confirm
Confirm a booking

http
PUT /api/bookings/{id}/cancel
Cancel a booking

File Operations
http
POST /api/fileoperations/save
Save bookings to JSON file (async)

http
POST /api/fileoperations/load
Load bookings from JSON file (async)

��� Running the Project
Development Mode
bash
# Navigate to API project
cd API

# Run the API
dotnet run

# The API will be available at:
# - http://localhost:5000
# - https://localhost:5001
# - Swagger UI: http://localhost:5000/swagger
Production Build
bash
# Publish the API
cd API
dotnet publish -c Release -o ./publish

# Run published version
cd publish
dotnet API.dll
��� Testing
Unit Testing
bash
# Run tests (when test projects are added)
dotnet test
API Testing
bash
# Using curl
curl http://localhost:5000/api/health
curl http://localhost:5000/api/conferencerooms
curl http://localhost:5000/api/bookings

# Using Swagger UI
# Open browser to: http://localhost:5000/swagger
Sample Data
The system includes sample data:

Conference rooms with various capacities and features

Sample bookings for testing

Async file operations with bookings.json

��� Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit changes (git commit -m 'Add AmazingFeature')

Push to branch (git push origin feature/AmazingFeature)

Open a Pull Request

��� License
This project is licensed under the MIT License - see the LICENSE file for details.

��� Acknowledgments
BitCube for the trainee program opportunity

Mentors and peers for guidance and support

The .NET community for excellent documentation and tools

��� Contact
Author: Siphosenkosi
GitHub: @mphosiphosenkosi-collab
Repository: https://github.com/mphosiphosenkosi-collab/Conference-Booking-Domain

Last Updated: $(date +%Y-%m-%d)
BitCube Trainee Program Project
