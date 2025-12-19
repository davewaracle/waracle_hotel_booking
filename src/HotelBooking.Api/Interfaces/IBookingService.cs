using HotelBooking.Api.Contracts;

namespace HotelBooking.Api.Interfaces;

public interface IBookingService
{
    Task<AvailabilityResponseDto> GetAvailabilityAsync(
        int hotelId,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests,
        CancellationToken ct);

    Task<CreateBookingResponse> CreateBookingAsync(
        CreateBookingRequest request,
        CancellationToken ct);

    Task<BookingDetailsDto?> GetBookingByReferenceAsync(
        string reference,
        CancellationToken ct);
}
