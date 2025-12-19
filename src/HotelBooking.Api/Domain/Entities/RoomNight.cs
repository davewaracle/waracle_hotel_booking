namespace HotelBooking.Api.Domain.Entities;

/// <summary>
/// One row per room per night. Unique(RoomId, NightDate) enforces "no double booking for any given night".
/// </summary>
public sealed class RoomNight
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public DateOnly NightDate { get; set; }
}
