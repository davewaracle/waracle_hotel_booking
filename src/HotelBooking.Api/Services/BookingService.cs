using HotelBooking.Api.Contracts;
using HotelBooking.Api.Domain.Entities;
using HotelBooking.Api.Infrastructure;
using HotelBooking.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelBooking.Api.Services;

public sealed class BookingService : IBookingService
{
    private readonly BookingDbContext _db;
    private readonly IBookingReferenceGenerator _refGen;
    private readonly ILogger<BookingService> _logger;

    public BookingService(BookingDbContext db, IBookingReferenceGenerator refGen, ILogger<BookingService> logger)
    {
        _db = db;
        _refGen = refGen;
        _logger = logger;
    }

    public async Task<AvailabilityResponseDto> GetAvailabilityAsync(
        int hotelId,
        DateOnly checkIn,
        DateOnly checkOut,
        int guests,
        CancellationToken ct)
    {
        BookingRules.ValidateDates(checkIn, checkOut);
        BookingRules.ValidateGuests(guests);

        var hotelExists = await _db.Hotels.AnyAsync(h => h.Id == hotelId, ct);
        if (!hotelExists) throw new KeyNotFoundException($"Hotel {hotelId} not found.");

        var rooms = await _db.Rooms
            .AsNoTracking()
            .Where(r => r.HotelId == hotelId && r.Capacity >= guests)
            .Where(r => !_db.RoomNights.Any(rn =>
                rn.RoomId == r.Id &&
                rn.NightDate >= checkIn &&
                rn.NightDate < checkOut))
            .OrderBy(r => r.RoomType)
            .ThenBy(r => r.RoomNumber)
            .Select(r => new RoomDto(r.Id, r.RoomNumber, r.RoomType, r.Capacity))
            .ToListAsync(ct);

        return new AvailabilityResponseDto(hotelId, checkIn, checkOut, guests, rooms);
    }

    public async Task<CreateBookingResponse> CreateBookingAsync(CreateBookingRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BookingRules.ValidateDates(request.CheckIn, request.CheckOut);
        BookingRules.ValidateGuests(request.Guests);

        // A tiny bit of "real-world" protection so someone can't book 10 years by accident.
        BookingRules.ValidateMaxStayLength(request.CheckIn, request.CheckOut, maxNights: 30);

        var hotel = await _db.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == request.HotelId, ct);

        if (hotel is null) throw new KeyNotFoundException($"Hotel {request.HotelId} not found.");

        _logger.LogInformation(
            "CreateBooking attempt HotelId={HotelId}, RoomType={RoomType}, CheckIn={CheckIn}, CheckOut={CheckOut}, Guests={Guests}",
            request.HotelId, request.RoomType, request.CheckIn, request.CheckOut, request.Guests);

        var candidateRoom = await FindFirstAvailableRoomAsync(request, ct);
        if (candidateRoom is null)
            throw new InvalidOperationException("No available room matches the request.");

        // Generate a human-friendly reference and (cheaply) try to avoid obvious collisions.
        // The unique index on Booking.Reference is still the real source of truth.
        var reference = await GenerateUniqueReferenceAsync(ct);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        try
        {
            var booking = new Booking
            {
                Reference = reference,
                HotelId = request.HotelId,
                RoomId = candidateRoom.Id,
                GuestCount = request.Guests,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                CreatedUtc = DateTime.UtcNow
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync(ct);

            foreach (var night in BookingRules.EnumerateNights(request.CheckIn, request.CheckOut))
            {
                _db.RoomNights.Add(new RoomNight
                {
                    RoomId = candidateRoom.Id,
                    BookingId = booking.Id,
                    NightDate = night
                });
            }

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            _logger.LogInformation("Booking created Reference={Reference}, RoomId={RoomId}", reference, candidateRoom.Id);

            // Keep response minimal – clients can look up details by reference.
            return new CreateBookingResponse(reference);
        }
        catch (DbUpdateException ex) when (DbErrors.IsUniqueConstraintViolation(ex))
        {
            await tx.RollbackAsync(ct);

            // Most likely: another request grabbed the same room-night(s).
            // In a larger system you might retry by selecting a different room.
            _logger.LogWarning(ex, "Booking conflict Reference={Reference}, RoomId={RoomId}", reference, candidateRoom.Id);
            throw new BookingConflictException("Room is no longer available for one or more nights.");
        }
    }

    public async Task<BookingDetailsDto?> GetBookingByReferenceAsync(string reference, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(reference)) return null;

        var booking = await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Nights)
            .FirstOrDefaultAsync(b => b.Reference == reference, ct);

        if (booking is null) return null;

        var room = await _db.Rooms.AsNoTracking().FirstAsync(r => r.Id == booking.RoomId, ct);
        var hotel = await _db.Hotels.AsNoTracking().FirstAsync(h => h.Id == booking.HotelId, ct);

        return new BookingDetailsDto(
            booking.Reference,
            hotel.Id,
            hotel.Name,
            room.Id,
            room.RoomNumber,
            room.RoomType,
            room.Capacity,
            booking.GuestCount,
            booking.CheckIn,
            booking.CheckOut,
            booking.CreatedUtc
        );
    }

    private async Task<Room?> FindFirstAvailableRoomAsync(CreateBookingRequest request, CancellationToken ct)
    {
        return await _db.Rooms
            .Where(r => r.HotelId == request.HotelId && r.RoomType == request.RoomType && r.Capacity >= request.Guests)
            .Where(r => !_db.RoomNights.Any(rn =>
                rn.RoomId == r.Id &&
                rn.NightDate >= request.CheckIn &&
                rn.NightDate < request.CheckOut))
            .OrderBy(r => r.RoomNumber)
            .FirstOrDefaultAsync(ct);
    }

    private async Task<string> GenerateUniqueReferenceAsync(CancellationToken ct)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = _refGen.Generate();

            var exists = await _db.Bookings.AsNoTracking().AnyAsync(b => b.Reference == candidate, ct);
            if (!exists) return candidate;
        }

        // Extremely unlikely – but it's nicer to fail loudly than spin forever.
        throw new InvalidOperationException("Unable to generate a unique booking reference.");
    }
}

public sealed class BookingConflictException : Exception
{
    public BookingConflictException(string message) : base(message) { }
}
