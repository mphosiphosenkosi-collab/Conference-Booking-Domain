using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using ConferenceRoomBooking.Domain.Exceptions;



namespace API.Middleware

{


    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;//call the function that will process the http request
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = exception switch
            {
                BookingConflictException => StatusCodes.Status422UnprocessableEntity,
                NoBookingsException => StatusCodes.Status422UnprocessableEntity,
                NoConferenceRoomsException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            var payload = new
            {
                error = exception.GetType().Name, //example: 400
                detail = exception.Message
            };
            return response.WriteAsync(JsonSerializer.Serialize(payload));
        }//HandleExceptionAsync

        public ExceptionHandlingMiddleware(RequestDelegate next) //pass function to the middleware
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) //context = request / response
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

    }


}
