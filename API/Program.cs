using ConferenceRoomBooking.Logic;
using Microsoft.OpenApi.Models; 
using API.Middleware;
using API.Data;
using API.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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



// Register BookingManager
builder.Services.AddSingleton<BookingManager>();

var app = builder.Build();

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

app.Run();
