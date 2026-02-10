using ConferenceRoomBooking.Logic;
using ConferenceRoomBooking.Logic.Persistence;
using Microsoft.OpenApi.Models; 
using API.Middleware;
using API.Data;
using API.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// create file store
var bookingStore = new BookingFileStore("bookings.json");

// register store
builder.Services.AddSingleton<IBookingStore>(bookingStore);

// register manager
builder.Services.AddSingleton<BookingManager>();


// Add services

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conference Booking API",
        Version = "v1"
    });
});

var app = builder.Build();

// load existing bookings from file
var manager = app.Services.GetRequiredService<BookingManager>();
await manager.InitializeAsync();

await SeedRoomsAsync(manager, "rooms.json");

// ✅ Use middleware AFTER app is built
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Booking API V1");
        c.RoutePrefix = string.Empty; // Optional: serve Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

async Task SeedRoomsAsync(BookingManager manager, string filePath)
{
    if (!File.Exists(filePath))
    {
        Console.WriteLine($"Room seed file '{filePath}' not found.");
        return;
    }

    var json = await File.ReadAllTextAsync(filePath);
    var rooms = System.Text.Json.JsonSerializer.Deserialize<List<ConferenceRoomBooking.Domain.ConferenceRoom>>(json);

    if (rooms != null)
    {
        foreach (var room in rooms)
        {
            if (!manager.GetRooms().Any(r => r.Id == room.Id))
            {
                manager.AddRoom(room); // You’ll need this method in BookingManager
            }
        }
    }
}


app.Run();
