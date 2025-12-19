using HotelBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Middleware;

public sealed class ApiExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(ILogger<ApiExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BookingConflictException ex)
        {
            _logger.LogInformation(ex, "Room unavailable");
            await WriteProblem(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // e.g. no available room for the request
            await WriteProblem(context, StatusCodes.Status409Conflict, ex.Message);
        }
    }

    private static async Task WriteProblem(HttpContext context, int statusCode, string detail)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var pd = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status409Conflict => "Conflict",
                _ => "Error"
            },
            Detail = detail
        };

        await context.Response.WriteAsJsonAsync(pd);
    }
}
