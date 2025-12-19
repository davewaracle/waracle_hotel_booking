using HotelBooking.Api.Domain.Entities;
using HotelBooking.Api.Infrastructure;
using HotelBooking.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Services;

public sealed class AdminDataService : IAdminDataService
{
    private readonly BookingDbContext _db;
    private readonly IBookingReferenceGenerator _refGen;

    public AdminDataService(BookingDbContext db, IBookingReferenceGenerator refGen)
    {
        _db = db;
        _refGen = refGen;
    }

    public async Task<AdminResetResult> ResetAsync(CancellationToken ct)
    {
        await _db.Database.MigrateAsync(ct);

        // Count first so we can report accurate numbers regardless of provider.
        var roomNights = await _db.RoomNights.CountAsync(ct);
        var bookings = await _db.Bookings.CountAsync(ct);
        var rooms = await _db.Rooms.CountAsync(ct);
        var hotels = await _db.Hotels.CountAsync(ct);

        // Order matters due to FK constraints.
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM RoomNights;", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Bookings;", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Rooms;", ct);
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Hotels;", ct);

        return new AdminResetResult(
            HotelsDeleted: hotels,
            RoomsDeleted: rooms,
            BookingsDeleted: bookings,
            RoomNightsDeleted: roomNights);
    }

    public async Task<AdminSeedResult> SeedAsync(CancellationToken ct)
    {
        // Use migrations rather than EnsureCreated so this works for both Sqlite and SqlServer.
        await _db.Database.MigrateAsync(ct);

        if (await _db.Hotels.AnyAsync(ct))
        {
            return new AdminSeedResult(
                Seeded: false,
                Message: "Database already contains data. Seed skipped.",
                HotelsCreated: 0,
                RoomsCreated: 0,
                BookingsCreated: 0,
                RoomNightsCreated: 0);
        }

        var hotelsCreated = 0;
        var roomsCreated = 0;
        var bookingsCreated = 0;
        var roomNightsCreated = 0;

        // Glasgow-flavoured seed data (just enough for Swagger testing)
        var grandCentral = new Hotel { Name = "Grand Central Hotel Glasgow" };
        var blythswood = new Hotel { Name = "Blythswood Square Hotel" };
        var dakota = new Hotel { Name = "Dakota Glasgow" };

        _db.Hotels.AddRange(grandCentral, blythswood, dakota);
        await _db.SaveChangesAsync(ct);
        hotelsCreated += 3;

        // 6 rooms: 2 singles (1), 2 doubles (2), 2 deluxe (4)
        SeedRooms(grandCentral.Id, "GC", _db);
        SeedRooms(blythswood.Id, "BS", _db);
        SeedRooms(dakota.Id, "DK", _db);

        await _db.SaveChangesAsync(ct);
        roomsCreated += 18;

        // Add one example booking so availability has something to react to.
        var exampleRoom = await _db.Rooms
            .Where(r => r.HotelId == blythswood.Id && r.RoomType == RoomType.Single)
            .OrderBy(r => r.RoomNumber)
            .FirstAsync(ct);

        var exampleBooking = new Booking
        {
            Reference = _refGen.Generate(),
            HotelId = blythswood.Id,
            RoomId = exampleRoom.Id,
            GuestCount = 1,
            CheckIn = new DateOnly(2026, 2, 1),
            CheckOut = new DateOnly(2026, 2, 3),
            CreatedUtc = DateTime.UtcNow
        };

        _db.Bookings.Add(exampleBooking);
        await _db.SaveChangesAsync(ct);
        bookingsCreated += 1;

        foreach (var night in BookingRules.EnumerateNights(exampleBooking.CheckIn, exampleBooking.CheckOut))
        {
            _db.RoomNights.Add(new RoomNight
            {
                RoomId = exampleRoom.Id,
                BookingId = exampleBooking.Id,
                NightDate = night
            });
            roomNightsCreated += 1;
        }

        await _db.SaveChangesAsync(ct);

        return new AdminSeedResult(
            Seeded: true,
            Message: "Seed completed.",
            HotelsCreated: hotelsCreated,
            RoomsCreated: roomsCreated,
            BookingsCreated: bookingsCreated,
            RoomNightsCreated: roomNightsCreated);
    }

    private static void SeedRooms(int hotelId, string prefix, BookingDbContext db)
    {
        var rooms = new List<Room>
        {
            new() { HotelId = hotelId, RoomNumber = $"{prefix}101", RoomType = RoomType.Single, Capacity = 1 },
            new() { HotelId = hotelId, RoomNumber = $"{prefix}102", RoomType = RoomType.Single, Capacity = 1 },
            new() { HotelId = hotelId, RoomNumber = $"{prefix}201", RoomType = RoomType.Double, Capacity = 2 },
            new() { HotelId = hotelId, RoomNumber = $"{prefix}202", RoomType = RoomType.Double, Capacity = 2 },
            new() { HotelId = hotelId, RoomNumber = $"{prefix}301", RoomType = RoomType.Deluxe, Capacity = 4 },
            new() { HotelId = hotelId, RoomNumber = $"{prefix}302", RoomType = RoomType.Deluxe, Capacity = 4 }
        };

        db.Rooms.AddRange(rooms);
    }
}
