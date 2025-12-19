namespace HotelBooking.Api.Domain.Entities;

public sealed class Hotel
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Room> Rooms { get; set; } = new();
}
