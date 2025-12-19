namespace HotelBooking.Api.Domain.Entities;

public sealed class Booking
{
    public int Id { get; set; }

    public required string Reference { get; set; } // unique booking reference
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int GuestCount { get; set; }

    // Check-in is inclusive; check-out is exclusive (nights are CheckIn <= d < CheckOut)
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public DateTime CreatedUtc { get; set; }

    public List<RoomNight> Nights { get; set; } = new();
}
