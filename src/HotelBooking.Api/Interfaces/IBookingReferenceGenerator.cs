namespace HotelBooking.Api.Interfaces;

public interface IBookingReferenceGenerator
{
    /// <summary>
    /// Generates a human-friendly booking reference (unique in practice; DB uniqueness enforced).
    /// </summary>
    string Generate();
}
