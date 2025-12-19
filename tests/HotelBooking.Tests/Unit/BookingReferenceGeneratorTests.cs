using System.Text.RegularExpressions;
using HotelBooking.Api.Services;
using Xunit;

namespace HotelBooking.Tests.Unit;

public class BookingReferenceGeneratorTests
{
    [Fact]
    public void Generate_ProducesExpectedFormat()
    {
        var gen = new BookingReferenceGenerator();

        var reference = gen.Generate();

        // GLA-YYYYMMDD-XXXXXX
        Assert.Matches(new Regex("^GLA-\\d{8}-[A-Z0-9]{6}$"), reference);
        Assert.Equal(4 + 8 + 1 + 6, reference.Length); // "GLA-" (4) + date (8) + "-" (1) + 6 chars
    }

    [Fact]
    public void Generate_IsUppercaseAndHyphenated()
    {
        var gen = new BookingReferenceGenerator();
        var reference = gen.Generate();

        Assert.Equal(reference.ToUpperInvariant(), reference);
        Assert.Equal(2, reference.Count(c => c == '-'));
    }

    [Fact]
    public void Generate_IsVeryLikelyUnique_InSmallBatch()
    {
        var gen = new BookingReferenceGenerator();

        var set = new HashSet<string>();
        for (var i = 0; i < 10_000; i++)
        {
            var r = gen.Generate();
            Assert.True(set.Add(r), $"Duplicate reference generated: {r}");
        }
    }
}
