using ConferenceRoomBooking.Logic;
using ConferenceRoomBooking.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON file path
var dataFilePath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "..", "ConferenceRoomBooking.Persistence", "Data", "bookings_data.json");

Console.WriteLine($"Data file path: {dataFilePath}");

// Register services
builder.Services.AddLogicServices();
builder.Services.AddPersistenceServices(dataFilePath);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
