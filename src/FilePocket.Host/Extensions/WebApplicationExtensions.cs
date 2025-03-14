using FilePocket.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.Shared.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FilePocketDbContext>();
        db.Database.Migrate();

        return app;
    }
}