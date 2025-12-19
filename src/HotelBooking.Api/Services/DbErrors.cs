using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Services;

/// <summary>
/// Small helper to map provider-specific unique-constraint errors to a single check.
/// </summary>
public static class DbErrors
{
    public static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // SQLite
        if (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqlite)
        {
            // SQLITE_CONSTRAINT (19) / SQLITE_CONSTRAINT_UNIQUE (2067) etc.
            return sqlite.SqliteErrorCode == 19 || sqlite.SqliteExtendedErrorCode == 2067;
        }

        // SQL Server
        if (ex.InnerException is SqlException sql)
        {
            // 2601: Cannot insert duplicate key row in object with unique index
            // 2627: Violation of UNIQUE KEY constraint
            return sql.Number is 2601 or 2627;
        }

        return false;
    }
}
