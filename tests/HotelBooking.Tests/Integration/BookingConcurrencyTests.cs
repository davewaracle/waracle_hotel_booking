using System.Net;
using System.Net.Http.Json;
using HotelBooking.Api.Contracts;
using HotelBooking.Api.Domain.Entities;
using Xunit;

namespace HotelBooking.Tests.Integration;

public sealed class BookingConcurrencyTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingConcurrencyTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Cannot_double_book_same_room_night()
    {
        // reset + seed
        var reset = await _client.PostAsync("/api/admin/reset", content: null);
        reset.EnsureSuccessStatusCode();

        var seed = await _client.PostAsync("/api/admin/seed", content: null);
        seed.EnsureSuccessStatusCode();

        // find hotel "Grand"
        var hotels = await _client.GetFromJsonAsync<List<HotelSummaryDto>>("/api/hotels?name=grand");
        Assert.NotNull(hotels);
        var hotelId = hotels!.Single().Id;

        var req = new CreateBookingRequest
        {
            HotelId = hotelId,
            RoomType = RoomType.Double,
            CheckIn = new DateOnly(2026, 1, 10),
            CheckOut = new DateOnly(2026, 1, 12),
            Guests = 2
        };

        // First booking should succeed
        var r1 = await _client.PostAsJsonAsync("/api/bookings", req);
        Assert.Equal(HttpStatusCode.Created, r1.StatusCode);

        // Second booking for same dates/type may or may not succeed depending on inventory (2 doubles seeded).
        // So we book both doubles, then ensure third attempt fails.
        var r2 = await _client.PostAsJsonAsync("/api/bookings", req);
        Assert.Equal(HttpStatusCode.Created, r2.StatusCode);

        var r3 = await _client.PostAsJsonAsync("/api/bookings", req);
        Assert.Equal(HttpStatusCode.Conflict, r3.StatusCode);
    }
}
