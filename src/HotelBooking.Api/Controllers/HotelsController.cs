using HotelBooking.Api.Contracts;
using HotelBooking.Api.Infrastructure;
using HotelBooking.Api.Interfaces;
using HotelBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/hotels")]
public sealed class HotelsController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly IBookingService _bookingService;

    public HotelsController(BookingDbContext db, IBookingService bookingService)
    {
        _db = db;
        _bookingService = bookingService;
    }

    /// <summary>Find hotels by name (case-insensitive, partial match).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<HotelSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FindByName([FromQuery] string name, CancellationToken ct)
    {
        name = (name ?? string.Empty).Trim();
        if (name.Length == 0) return Ok(Array.Empty<HotelSummaryDto>());

        var hotels = await _db.Hotels
            .AsNoTracking()
            .Where(h => EF.Functions.Like(h.Name.ToLower(), $"%{name.ToLower()}%"))
            .OrderBy(h => h.Name)
            .Select(h => new HotelSummaryDto(h.Id, h.Name))
            .ToListAsync(ct);

        return Ok(hotels);
    }

    /// <summary>Find available rooms in a hotel between two dates for a given number of guests.</summary>
    [HttpGet("{hotelId:int}/availability")]
    [ProducesResponseType(typeof(AvailabilityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailability(
        [FromRoute] int hotelId,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] int guests,
        CancellationToken ct)
    {
        var result = await _bookingService.GetAvailabilityAsync(hotelId, checkIn, checkOut, guests, ct);
        return Ok(result);
    }
}
