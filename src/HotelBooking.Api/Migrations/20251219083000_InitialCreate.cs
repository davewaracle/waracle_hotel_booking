using HotelBooking.Api.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Api.Migrations;

/// <summary>
/// Initial schema for Hotels, Rooms, Bookings and RoomNights.
/// </summary>
[DbContext(typeof(BookingDbContext))]
[Migration("20251219083000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Hotels",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Hotels", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Rooms",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                HotelId = table.Column<int>(type: "INTEGER", nullable: false),
                RoomNumber = table.Column<string>(type: "TEXT", nullable: false),
                RoomType = table.Column<int>(type: "INTEGER", nullable: false),
                Capacity = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rooms", x => x.Id);
                table.ForeignKey(
                    name: "FK_Rooms_Hotels_HotelId",
                    column: x => x.HotelId,
                    principalTable: "Hotels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Bookings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Reference = table.Column<string>(type: "TEXT", nullable: false),
                HotelId = table.Column<int>(type: "INTEGER", nullable: false),
                RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                GuestCount = table.Column<int>(type: "INTEGER", nullable: false),
                CheckIn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                CheckOut = table.Column<DateOnly>(type: "TEXT", nullable: false),
                CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bookings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Bookings_Hotels_HotelId",
                    column: x => x.HotelId,
                    principalTable: "Hotels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Bookings_Rooms_RoomId",
                    column: x => x.RoomId,
                    principalTable: "Rooms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "RoomNights",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                BookingId = table.Column<int>(type: "INTEGER", nullable: false),
                NightDate = table.Column<DateOnly>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoomNights", x => x.Id);
                table.ForeignKey(
                    name: "FK_RoomNights_Bookings_BookingId",
                    column: x => x.BookingId,
                    principalTable: "Bookings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RoomNights_Rooms_RoomId",
                    column: x => x.RoomId,
                    principalTable: "Rooms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_HotelId",
            table: "Bookings",
            column: "HotelId");

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_RoomId",
            table: "Bookings",
            column: "RoomId");

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_Reference",
            table: "Bookings",
            column: "Reference",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Hotels_Name",
            table: "Hotels",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_RoomNights_BookingId",
            table: "RoomNights",
            column: "BookingId");

        migrationBuilder.CreateIndex(
            name: "IX_RoomNights_RoomId_NightDate",
            table: "RoomNights",
            columns: new[] { "RoomId", "NightDate" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Rooms_HotelId",
            table: "Rooms",
            column: "HotelId");

        migrationBuilder.CreateIndex(
            name: "IX_Rooms_HotelId_RoomNumber",
            table: "Rooms",
            columns: new[] { "HotelId", "RoomNumber" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "RoomNights");
        migrationBuilder.DropTable(name: "Bookings");
        migrationBuilder.DropTable(name: "Rooms");
        migrationBuilder.DropTable(name: "Hotels");
    }
}
