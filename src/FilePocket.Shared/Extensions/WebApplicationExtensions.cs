using FilePocket.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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