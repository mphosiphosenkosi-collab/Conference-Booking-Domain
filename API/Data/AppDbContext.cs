using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using API.Auth;
using ConferenceRoomBooking.Domain;
using ConferenceRoomBooking.Domain.Enums;

namespace API.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        // DbSets for your domain entities
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBookingEntity(modelBuilder);
            ConfigureConferenceRoomEntity(modelBuilder);
            ConfigureSessionEntity(modelBuilder);
            
            SeedData(modelBuilder);
        }

        private void ConfigureBookingEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();
                
                entity.Property(b => b.UserEmail)
                    .IsRequired()
                    .HasMaxLength(256);
                
                entity.Property(b => b.StartTime).IsRequired();
                entity.Property(b => b.EndTime).IsRequired();
                
                entity.Property(b => b.Status)
                    .IsRequired()
                    .HasConversion<string>(); // Store enum as string
                
                // NEW: Assignment properties
                entity.Property(b => b.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("DATETIME('now')"); // SQLite syntax
                
                entity.Property(b => b.CancelledAt)
                    .IsRequired(false); // Nullable
                
                // Index for performance
                entity.HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime });
                entity.HasIndex(b => b.UserEmail);
            });
        }

        private void ConfigureConferenceRoomEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConferenceRoom>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(r => r.Type)
                    .IsRequired()
                    .HasConversion<string>(); // Store enum as string
                
                entity.Property(r => r.Capacity).IsRequired();
                
                // Convert List<string> to comma-separated string for SQLite
                entity.Property(r => r.Features)
                    .HasConversion(
                        v => string.Join(';', v),
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                // NEW: Assignment properties
                entity.Property(r => r.Location)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasDefaultValue("Unknown");
                
                entity.Property(r => r.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                // Indexes
                entity.HasIndex(r => r.Name).IsUnique();
                entity.HasIndex(r => r.IsActive);
            });
        }

        private void ConfigureSessionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
                
                entity.Property(s => s.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(s => s.Description)
                    .HasMaxLength(1000);
                
                entity.Property(s => s.Capacity).IsRequired();
                entity.Property(s => s.StartTime).IsRequired();
                entity.Property(s => s.EndTime).IsRequired();
                
                entity.Property(s => s.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);
                
                // Foreign key to ConferenceRoom
                entity.HasOne<ConferenceRoom>()
                    .WithMany()
                    .HasForeignKey(s => s.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Indexes
                entity.HasIndex(s => s.RoomId);
                entity.HasIndex(s => s.StartTime);
                entity.HasIndex(s => s.IsActive);
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed at least one active room (assignment requirement)
            var activeRoom = new ConferenceRoom(
                id: 1,
                name: "Main Conference Hall",
                type: RoomType.Conference,
                capacity: 100,
                features: new[] { "Projector", "Whiteboard", "Video Conferencing" },
                location: "Building A, Floor 3"
            );
            
            // Seed at least one inactive room (assignment requirement)
            var inactiveRoom = new ConferenceRoom(
                id: 2,
                name: "Old Boardroom",
                type: RoomType.Boardroom,
                capacity: 12,
                features: new[] { "Whiteboard" },
                location: "Building B, Floor 1"
            );
            // Manually set IsActive to false since Deactivate() method can't be called in HasData
            modelBuilder.Entity<ConferenceRoom>().HasData(
                activeRoom,
                new { inactiveRoom.Id, inactiveRoom.Name, inactiveRoom.Type, 
                      inactiveRoom.Capacity, inactiveRoom.Features, 
                      inactiveRoom.Location, IsActive = false }
            );

            // Seed at least one session with valid time range (assignment requirement)
            var futureDate = DateTime.UtcNow.AddDays(7).Date;
            var session = new Session(
                id: 1,
                title: "Annual Strategy Meeting",
                description: "Company-wide strategy planning session",
                capacity: 80,
                startTime: new DateTime(futureDate.Year, futureDate.Month, futureDate.Day, 9, 0, 0, DateTimeKind.Utc),
                endTime: new DateTime(futureDate.Year, futureDate.Month, futureDate.Day, 17, 0, 0, DateTimeKind.Utc),
                roomId: 1
            );

            modelBuilder.Entity<Session>().HasData(session);

            // Seed at least one booking in non-default status (assignment requirement)
            var bookingDate = DateTime.UtcNow.AddDays(1).Date;
            modelBuilder.Entity<Booking>().HasData(
                new
                {
                    Id = 1,
                    RoomId = 1,
                    UserEmail = "admin@ProjectT.com",
                    StartTime = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 10, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(bookingDate.Year, bookingDate.Month, bookingDate.Day, 12, 0, 0, DateTimeKind.Utc),
                    Status = BookingStatus.Confirmed, // Non-default status
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    CancelledAt = (DateTime?)null
                }
            );
        }
    }
}