namespace HotelBooking.Api.Services;

/// <summary>
/// Central place for booking-related validation and date handling.
/// This keeps controllers/services readable and makes unit testing easier.
/// </summary>
public static class BookingRules
{
    public static void ValidateDates(DateOnly checkIn, DateOnly checkOut)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("CheckOut must be after CheckIn.");
    }

    public static void ValidateGuests(int guests)
    {
        if (guests <= 0)
            throw new ArgumentOutOfRangeException(nameof(guests), "Guests must be at least 1.");
    }

    public static void ValidateMaxStayLength(DateOnly checkIn, DateOnly checkOut, int maxNights)
    {
        var nights = checkOut.DayNumber - checkIn.DayNumber;
        if (nights > maxNights)
            throw new ArgumentException($"Stay length cannot exceed {maxNights} nights.");
    }

    /// <summary>
    /// Enumerates each night date for an inclusive/exclusive stay: [checkIn, checkOut).
    /// </summary>
    public static IEnumerable<DateOnly> EnumerateNights(DateOnly checkIn, DateOnly checkOut)
    {
        ValidateDates(checkIn, checkOut);

        for (var d = checkIn; d < checkOut; d = d.AddDays(1))
            yield return d;
    }
}
