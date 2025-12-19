using HotelBooking.Api.Contracts;
using HotelBooking.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>Book a room (server selects a room of the requested type that fits the guest count).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var created = await _bookingService.CreateBookingAsync(request, ct);
        return CreatedAtAction(nameof(GetByReference), new { reference = created.Reference }, created);
    }

    /// <summary>Get booking details by booking reference.</summary>
    [HttpGet("{reference}")]
    [ProducesResponseType(typeof(BookingDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByReference([FromRoute] string reference, CancellationToken ct)
    {
        var booking = await _bookingService.GetBookingByReferenceAsync(reference, ct);
        return booking is null
            ? NotFound(new ProblemDetails
            {
                Title = "Booking not found",
                Detail = $"No booking exists with reference '{reference}'."
            })
            : Ok(booking);
    }
}
