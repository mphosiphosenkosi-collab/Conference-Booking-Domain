using Microsoft.Extensions.DependencyInjection;
using ConferenceRoomBooking.Persistence.Repositories;
using ConferenceRoomBooking.Persistence.Stores;
using ConferenceRoomBooking.Logic.Interfaces;

namespace ConferenceRoomBooking.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string dataFilePath)
    {
        services.AddSingleton<JsonDataStore>(provider => new JsonDataStore(dataFilePath));
        
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        
        return services;
    }
}
