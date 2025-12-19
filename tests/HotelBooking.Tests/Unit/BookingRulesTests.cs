using HotelBooking.Api.Services;
using Xunit;

namespace HotelBooking.Tests.Unit;

public class BookingRulesTests
{
    [Fact]
    public void EnumerateNights_IsInclusiveExclusive()
    {
        var checkIn = new DateOnly(2026, 1, 10);
        var checkOut = new DateOnly(2026, 1, 12);

        var nights = BookingRules.EnumerateNights(checkIn, checkOut).ToArray();

        Assert.Equal(2, nights.Length);
        Assert.Equal(new DateOnly(2026, 1, 10), nights[0]);
        Assert.Equal(new DateOnly(2026, 1, 11), nights[1]);
    }

    [Theory]
    [InlineData(2026, 1, 10, 2026, 1, 10)]
    [InlineData(2026, 1, 11, 2026, 1, 10)]
    public void ValidateDates_Throws_When_Invalid(int inY, int inM, int inD, int outY, int outM, int outD)
    {
        var checkIn = new DateOnly(inY, inM, inD);
        var checkOut = new DateOnly(outY, outM, outD);

        Assert.Throws<ArgumentException>(() => BookingRules.ValidateDates(checkIn, checkOut));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ValidateGuests_Throws_When_Invalid(int guests)
    {
        Assert.ThrowsAny<ArgumentOutOfRangeException>(() => BookingRules.ValidateGuests(guests));
    }

    [Fact]
    public void ValidateMaxStayLength_Throws_When_TooLong()
    {
        var checkIn = new DateOnly(2026, 1, 1);
        var checkOut = new DateOnly(2026, 2, 15);

        Assert.Throws<ArgumentException>(() => BookingRules.ValidateMaxStayLength(checkIn, checkOut, maxNights: 30));
    }
}
