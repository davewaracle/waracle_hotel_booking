namespace HotelBooking.Api.Domain.Entities;

public sealed class Room
{
    public int Id { get; set; }

    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    public required string RoomNumber { get; set; } // e.g. "101"
    public RoomType RoomType { get; set; }

    public int Capacity { get; set; } // Single=1, Double=2, Deluxe=4
}
