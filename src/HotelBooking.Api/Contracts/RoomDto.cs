using HotelBooking.Api.Domain.Entities;

namespace HotelBooking.Api.Contracts;

public sealed record RoomDto(
    int Id,
    string RoomNumber,
    RoomType RoomType,
    int Capacity
);
