# Hotel Booking API (ASP.NET Core + EF Core)

A small RESTful API for hotel room availability + bookings, built as a take-home style backend exercise.

Local dev uses **SQLite** for zero-setup. It’s also **Azure SQL / SQL Server-ready** via the same EF Core model + migrations.

## Quick start

```bash
# from repo root
dotnet restore
dotnet run --project src/HotelBooking.Api
```

On first run the API will apply EF Core migrations automatically.
For testing, seed/reset is exposed explicitly via admin endpoints (per the brief).

Open Swagger:
- `https://localhost:xxxx/swagger`

## Testing flow (in Swagger)

The admin endpoints are intentionally explicit (per spec):
- `reset` returns deletion counts
- `seed` is idempotent (safe to call repeatedly)

1. `POST /api/admin/reset`
2. `POST /api/admin/seed` (seeds Glasgow-themed hotels + rooms)
3. `GET /api/hotels?name=grand` (find “Grand Central Hotel Glasgow”)
4. `GET /api/hotels/{hotelId}/availability?...` (check rooms)
5. `POST /api/bookings` (book a room)
6. `GET /api/bookings/{reference}` (fetch booking details)

## Business rules covered

- Hotels have 3 room types: `Single`, `Double`, `Deluxe` (**Deluxe capacity = 4**)
- Hotels have **6 rooms** (seeded as: 2 singles, 2 doubles, 2 deluxes)
- A room cannot be double-booked for any given night
- Bookings never require changing rooms (1 booking = 1 room for whole stay)
- Booking references are unique
- Room capacity enforced (`guests <= capacity`)

## Design notes (the bits that usually bite)

### Dates are `CheckIn` inclusive / `CheckOut` exclusive
If you book 10th → 12th, you occupy nights **10th and 11th**.
This keeps availability math simple and avoids off-by-one surprises.

### Why `RoomNight` exists
`RoomNight` stores **one row per room per night** and has a unique index on `(RoomId, NightDate)`.

That means:
- we can reliably prevent double-booking **even under concurrency**
- booking a stay is “reserve all nights for this room” in a single transaction

It’s intentionally a bit denormalised, but very practical for correctness.

### Booking reference strategy (keep it boring)
The booking reference is formatted as `GLA-YYYYMMDD-XXXXXX`.

This is intentionally simple:
- it’s readable and easy to quote over the phone
- it’s easy to grep in logs (prefix + date)
- it avoids bespoke encodings/check-digits that don’t buy us much in this domain

Uniqueness is enforced by a database unique index on `Booking.Reference`. The service does a cheap
existence check before insert to avoid an obvious collision, and the DB remains the source of truth.

## Seeding data (Glasgow flavour)

The seed endpoint creates a few Glasgow-themed hotels (e.g. “Grand Central Hotel Glasgow”, “Blythswood Square Hotel”, “Dakota Glasgow”),
each with 6 rooms, and one example booking (kept away from the test dates).

## Database configuration

### Default (SQLite)
`appsettings.json` uses a local SQLite file DB by default.

### Azure SQL / SQL Server
Set a SQL Server connection string and switch provider in `Program.cs` (see the `// SQL Server` comment).
In Azure App Service you’d typically set:
- `ConnectionStrings__Default`

## Tests

`tests/HotelBooking.Tests` contains a basic integration test that:
- resets + seeds
- books both available doubles for a date range
- verifies the third attempt returns `409 Conflict`

Run:
```bash
dotnet test
```

## Known limitations / next steps

- No authentication (per brief)
- No cancellation/modification endpoints (easy to add by removing `RoomNight` rows + marking booking cancelled)
- No pagination on search endpoints (not needed for the exercise)
