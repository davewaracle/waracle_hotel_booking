namespace HotelBooking.Api.Interfaces;

public interface IAdminDataService
{
    Task<AdminResetResult> ResetAsync(CancellationToken ct);
    Task<AdminSeedResult> SeedAsync(CancellationToken ct);
}

/// <summary>
/// Result returned by the reset endpoint.
/// </summary>
public sealed record AdminResetResult(
    int HotelsDeleted,
    int RoomsDeleted,
    int BookingsDeleted,
    int RoomNightsDeleted);

/// <summary>
/// Result returned by the seed endpoint.
/// </summary>
public sealed record AdminSeedResult(
    bool Seeded,
    string Message,
    int HotelsCreated,
    int RoomsCreated,
    int BookingsCreated,
    int RoomNightsCreated);
