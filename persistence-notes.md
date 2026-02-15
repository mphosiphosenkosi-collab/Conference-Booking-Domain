# Persistence & EF Core — Notes

## In-memory storage — why not production
In-memory storage data can be lost on restart or if the system crashes, and lacks durability
It also limits capacity and provides poor security. 
Suitable only for tests or quick prototypes.

## What DbContext represents
DbContext is EF Core’s unit-of-work. It exposes DbSet<T> collections that map domain types to tables. 
In this project AppDbContext defines the schema and entity configuration.

## How EF Core fits into the architecture
EF Core is the persistence/ORM layer between your domain/logic (RoomManager,BookingManager) and the physical database. 
Controllers/services use AppDbContext.

## How this prepares the system for:
- Relationships: This allows connected tables to be linked to one another in the background, which makes for better security, as well as easier handling of queries
- Ownership: The databases are easy to find, create and maintain, so data can be easliy sorted and found
- Frontend usage: Error handling and encapsulated http responses can be easily mapped to the front end of the application

