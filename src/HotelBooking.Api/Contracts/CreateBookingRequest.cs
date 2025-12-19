using HotelBooking.Api.Domain.Entities;

namespace HotelBooking.Api.Contracts;

public sealed class CreateBookingRequest
{
    public int HotelId { get; set; }
    public RoomType RoomType { get; set; }

    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }

    public int Guests { get; set; }
}
