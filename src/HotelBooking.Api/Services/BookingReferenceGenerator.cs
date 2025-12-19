using HotelBooking.Api.Interfaces;

namespace HotelBooking.Api.Services;

/// <summary>
/// Simple, human-friendly booking reference.
///
/// Format: GLA-YYYYMMDD-XXXXXX
/// - GLA: small nod to the Glasgow-themed seed data (and useful for log searching)
/// - YYYYMMDD: creation date (helps support/debugging)
/// - XXXXXX: short GUID fragment (enough entropy for practical uniqueness)
///
/// Uniqueness is enforced by a DB unique index. The booking service does a quick existence check
/// to avoid an obvious collision, then the DB is still the source of truth.
/// </summary>
public sealed class BookingReferenceGenerator : IBookingReferenceGenerator
{
    private const string Prefix = "GLA";

    public string Generate()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");

        var suffix = Guid.NewGuid()
            .ToString("N")
            .Substring(0, 6)
            .ToUpperInvariant();

        return $"{Prefix}-{date}-{suffix}";
    }
}
