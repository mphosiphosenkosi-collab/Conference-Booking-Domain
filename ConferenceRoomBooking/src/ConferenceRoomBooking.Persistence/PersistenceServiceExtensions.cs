using Microsoft.Extensions.DependencyInjection;
using ConferenceRoomBooking.Persistence.Repositories;
using ConferenceRoomBooking.Persistence.Stores;
using ConferenceRoomBooking.Logic.Interfaces;  // Use the Logic interface

namespace ConferenceRoomBooking.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string dataFilePath)
    {
        // Register JsonDataStore as singleton
        services.AddSingleton<JsonDataStore>(provider => new JsonDataStore(dataFilePath));
        
        // Register repositories - now using Logic.Interfaces.IBookingRepository
        services.AddScoped<IBookingRepository, BookingRepository>();
        
        return services;
    }
    
    // Overload for when no path is provided (uses default)
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        // Default data file path
        var defaultPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "ConferenceRoomBooking.Persistence", "Data", "bookings_data.json");
        
        return AddPersistenceServices(services, defaultPath);
    }
}
