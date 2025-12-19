using HotelBooking.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IAdminDataService _admin;

    public AdminController(IAdminDataService admin)
    {
        _admin = admin;
    }

    /// <summary>
    /// Reset all data (testing helper).
    /// </summary>
    /// <remarks>
    /// Intended for Swagger/manual testing. Returns counts so it's obvious what changed.
    /// </remarks>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(AdminResetResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminResetResult>> Reset(CancellationToken ct)
    {
        var result = await _admin.ResetAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// Seed minimal data for testing (testing helper).
    /// </summary>
    /// <remarks>
    /// Idempotent: if data already exists, seeding is skipped and the response explains why.
    /// </remarks>
    [HttpPost("seed")]
    [ProducesResponseType(typeof(AdminSeedResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminSeedResult>> Seed(CancellationToken ct)
    {
        var result = await _admin.SeedAsync(ct);
        return Ok(result);
    }
}
