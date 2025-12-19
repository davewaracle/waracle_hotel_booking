using HotelBooking.Api.Infrastructure;
using HotelBooking.Api.Interfaces;
using HotelBooking.Api.Middleware;
using HotelBooking.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Booking API", Version = "v1" });
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
});

builder.Services.AddTransient<ApiExceptionMiddleware>();

// EF Core: SQLite by default, SQL Server when configured.
// appsettings.json:
//   Database:Provider = "Sqlite" | "SqlServer"
var provider = (builder.Configuration["Database:Provider"] ?? "Sqlite").Trim();

builder.Services.AddDbContext<BookingDbContext>(options =>
{
    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        var cs = builder.Configuration.GetConnectionString("SqlServer")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:SqlServer");
        options.UseSqlServer(cs);
    }
    else
    {
        var cs = builder.Configuration.GetConnectionString("Sqlite")
                 ?? "Data Source=hotelbooking.db";
        options.UseSqlite(cs);
    }
});

builder.Services.AddScoped<IBookingReferenceGenerator, BookingReferenceGenerator>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAdminDataService, AdminDataService>();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Startup convenience: auto-apply migrations (handy for local dev and Swagger testing).
if (app.Configuration.GetValue<bool>("Database:AutoMigrate"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();

// Expose Program for WebApplicationFactory in tests
public partial class Program { }
