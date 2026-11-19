using Microsoft.EntityFrameworkCore;

namespace RoomHub.Api.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    // Creates the SQLite schema on first start, so a clean clone runs without manual steps.
    public static void InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RoomHubDbContext>();
        db.Database.EnsureCreated();
    }
}
