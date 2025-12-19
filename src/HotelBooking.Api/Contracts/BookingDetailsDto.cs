using HotelBooking.Api.Domain.Entities;

namespace HotelBooking.Api.Contracts;

public sealed record BookingDetailsDto(
    string Reference,
    int HotelId,
    string HotelName,
    int RoomId,
    string RoomNumber,
    RoomType RoomType,
    int RoomCapacity,
    int GuestCount,
    DateOnly CheckIn,
    DateOnly CheckOut,
    DateTime CreatedUtc
);
