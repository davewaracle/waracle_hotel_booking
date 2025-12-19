using HotelBooking.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Infrastructure;

public sealed class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<RoomNight> RoomNights => Set<RoomNight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.Name);

        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.HotelId, r.RoomNumber })
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.Reference)
            .IsUnique();

        modelBuilder.Entity<RoomNight>()
            .HasIndex(rn => new { rn.RoomId, rn.NightDate })
            .IsUnique();

        // Relationships
        modelBuilder.Entity<Hotel>()
            .HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Room>()
            .HasMany<RoomNight>()
            .WithOne(rn => rn.Room)
            .HasForeignKey(rn => rn.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasMany(b => b.Nights)
            .WithOne(rn => rn.Booking)
            .HasForeignKey(rn => rn.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

