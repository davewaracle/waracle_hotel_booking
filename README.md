# 🏨 Hotel Booking API – Glasgow (Human Spec)

A simple, clean ASP.NET Core Web API that exposes hotel search, availability, and booking functionality, along with admin endpoints to reset and seed data.

This solution is designed to be:

- Easy to run locally
- Deterministic (reset + seed supported)
- Testable via Postman without extra tooling
- Aligned with a clear, human-readable API spec

---

## 📦 Solution Overview

**Core capabilities**

- Search hotels by name
- Check room availability for a date range
- Create a booking
- Retrieve a booking by reference
- Reset and seed the database via admin endpoints

**Key projects**

- `HotelBooking.Api` – ASP.NET Core Web API
- In-memory or lightweight persistence (seed/reset supported by spec)

---

## 🚀 Running the API Locally

### Prerequisites

- .NET SDK (as defined by the solution)
- No external dependencies required

### Run

```bash
dotnet run --project src/HotelBooking.Api
```

Default base URL:

```
http://localhost:5000
```

---

## 🔐 Authentication

No authentication is required (by design, per spec).

---

## 🔁 API Endpoints

### Admin

| Method | Endpoint           | Description                  |
| ------ | ------------------ | ---------------------------- |
| POST   | `/api/admin/reset` | Clears all data              |
| POST   | `/api/admin/seed`  | Seeds initial hotels & rooms |

---

### Hotels

| Method | Endpoint                             | Description        |
| ------ | ------------------------------------ | ------------------ |
| GET    | `/api/hotels?name=Glasgow`           | Search hotels      |
| GET    | `/api/hotels/{hotelId}/availability` | Check availability |

Query parameters:

- `checkIn` – yyyy-MM-dd
- `checkOut` – yyyy-MM-dd
- `guests` – integer

---

### Bookings

| Method | Endpoint                    | Description    |
| ------ | --------------------------- | -------------- |
| POST   | `/api/bookings`             | Create booking |
| GET    | `/api/bookings/{reference}` | Get booking    |

---

## 🧪 Testing with Postman

Import the provided Postman collection:

- `HotelBooking.Api.postman_collection.json`

### Collection Variables (examples)

| Variable        | Example               |
| --------------- | --------------------- |
| baseUrl         | http://localhost:5201 |
| hotelSearchName | Glasgow               |
| checkIn         | 2026-02-01            |
| checkOut        | 2026-02-03            |
| guests          | 1                     |
| roomType        | Single                |

---

## ▶️ Recommended Test Flow

1. **POST** `/api/admin/reset`
2. **POST** `/api/admin/seed`
3. **GET** `/api/hotels?name=Glasgow`
4. **GET** `/api/hotels/{hotelId}/availability`
5. **POST** `/api/bookings`
6. **GET** `/api/bookings/{bookingReference}`

---

## 🧠 Notes

- Dates use ISO format
- Booking reference is generated server-side
- Reset/Seed endpoints exist purely for testing
- No auth, rate-limiting, or persistence guarantees

---

Designed for clarity, testability, and human-readable behaviour.
