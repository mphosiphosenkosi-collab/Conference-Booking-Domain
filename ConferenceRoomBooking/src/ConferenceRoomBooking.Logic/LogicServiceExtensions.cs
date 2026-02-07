using Microsoft.Extensions.DependencyInjection;
using ConferenceRoomBooking.Logic.Interfaces;
using ConferenceRoomBooking.Logic.Services;

namespace ConferenceRoomBooking.Logic;

public static class LogicServiceExtensions
{
    public static IServiceCollection AddLogicServices(this IServiceCollection services)
    {
        // Register business logic services
        services.AddScoped<IBookingService, BookingService>();
        
        return services;
    }
}
