// Program.cs - Application Host Configuration
// Assignment 2.1 Requirement 2: Configure HTTP pipeline, enable controllers, register services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// 1. Enable controllers (Requirement 2)
builder.Services.AddControllers();

// 2. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Register application services (Requirement 2)
// TODO: Add service registrations when controllers are created

var app = builder.Build();

// Configure the HTTP request pipeline (Requirement 2)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


