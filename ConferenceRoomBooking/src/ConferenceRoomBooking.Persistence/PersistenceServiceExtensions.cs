using Microsoft.Extensions.DependencyInjection;
using ConferenceRoomBooking.Persistence.Repositories;
using ConferenceRoomBooking.Persistence.Stores;
using ConferenceRoomBooking.Logic.Interfaces;

namespace ConferenceRoomBooking.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string dataFilePath)
    {
        // Register JsonDataStore as singleton (single instance for entire app)
        services.AddSingleton<JsonDataStore>(provider => new JsonDataStore(dataFilePath));
        
        // Register repositories
        services.AddScoped<IBookingRepository, BookingRepository>();
        
        return services;
    }
}
