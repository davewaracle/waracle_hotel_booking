using HotelBooking.Api.Domain.Entities;
using HotelBooking.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelBooking.Api.Migrations;

[DbContext(typeof(BookingDbContext))]
public partial class BookingDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        #pragma warning disable 612, 618

        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.11");

        modelBuilder.Entity<Hotel>(b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property<string>("Name")
                .IsRequired()
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("Name");

            b.ToTable("Hotels");
        });

        modelBuilder.Entity<Room>(b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property<int>("Capacity")
                .HasColumnType("INTEGER");

            b.Property<int>("HotelId")
                .HasColumnType("INTEGER");

            b.Property<string>("RoomNumber")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("RoomType")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.HasIndex("HotelId");

            b.HasIndex("HotelId", "RoomNumber")
                .IsUnique();

            b.ToTable("Rooms");
        });

        modelBuilder.Entity<Booking>(b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property<DateOnly>("CheckIn")
                .HasColumnType("TEXT");

            b.Property<DateOnly>("CheckOut")
                .HasColumnType("TEXT");

            b.Property<DateTime>("CreatedUtc")
                .HasColumnType("TEXT");

            b.Property<int>("GuestCount")
                .HasColumnType("INTEGER");

            b.Property<int>("HotelId")
                .HasColumnType("INTEGER");

            b.Property<string>("Reference")
                .IsRequired()
                .HasColumnType("TEXT");

            b.Property<int>("RoomId")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.HasIndex("HotelId");

            b.HasIndex("RoomId");

            b.HasIndex("Reference")
                .IsUnique();

            b.ToTable("Bookings");
        });

        modelBuilder.Entity<RoomNight>(b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property<int>("BookingId")
                .HasColumnType("INTEGER");

            b.Property<DateOnly>("NightDate")
                .HasColumnType("TEXT");

            b.Property<int>("RoomId")
                .HasColumnType("INTEGER");

            b.HasKey("Id");

            b.HasIndex("BookingId");

            b.HasIndex("RoomId", "NightDate")
                .IsUnique();

            b.ToTable("RoomNights");
        });

        modelBuilder.Entity<Booking>(b =>
        {
            b.HasOne<Hotel>()
                .WithMany()
                .HasForeignKey("HotelId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne<Room>()
                .WithMany()
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<Room>(b =>
        {
            b.HasOne<Hotel>()
                .WithMany("Rooms")
                .HasForeignKey("HotelId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<RoomNight>(b =>
        {
            b.HasOne<Booking>()
                .WithMany("Nights")
                .HasForeignKey("BookingId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne<Room>()
                .WithMany()
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        #pragma warning restore 612, 618
    }
}
