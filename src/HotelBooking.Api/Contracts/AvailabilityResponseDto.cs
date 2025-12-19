namespace HotelBooking.Api.Contracts;

public sealed record AvailabilityResponseDto(
    int HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Guests,
    IReadOnlyList<RoomDto> Rooms
);
