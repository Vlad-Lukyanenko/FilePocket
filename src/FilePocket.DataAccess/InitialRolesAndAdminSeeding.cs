﻿using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FilePocket.DataAccess;

public class InitialRolesAndAdminSeeding : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminSeedingDataModel _adminSeedingData;

    public InitialRolesAndAdminSeeding(IServiceProvider serviceProvider, IOptions<AdminSeedingDataModel> options)
    {
        _serviceProvider = serviceProvider;
        _adminSeedingData = options.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken token)
    {
        using var scope = _serviceProvider.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<FilePocketDbContext>();

        await SeedRoles(dbContext, token);
        await SeedAdmin(dbContext, token);
    }

    private static async Task SeedRoles(FilePocketDbContext dbContext, CancellationToken token)
    {
        if (await dbContext.Roles.AnyAsync(token))
        {
            return;
        }

        dbContext.AddRange(
            new Role { Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER" },
            new Role { Id = Guid.NewGuid(), Name = "Administrator", NormalizedName = "ADMINISTRATOR" });

        await dbContext.SaveChangesAsync(token);
    }

    private async Task SeedAdmin(FilePocketDbContext dbContext, CancellationToken token)
    {
        if (await dbContext.Users.AnyAsync(x => x.UserName!.Equals(_adminSeedingData.UserName), token))
        {
            return;
        }

        var admin = new User
        {
            Email = _adminSeedingData.Email,
            EmailConfirmed = true,
            FirstName = _adminSeedingData.FirstName,
            LastName = _adminSeedingData.LastName,
            UserName = _adminSeedingData.UserName,
            NormalizedUserName = _adminSeedingData.UserName!.ToUpper(),
            NormalizedEmail = _adminSeedingData.Email!.ToUpper(),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var passwordHasher = new PasswordHasher<User>();
        admin.PasswordHash = passwordHasher.HashPassword(admin, _adminSeedingData.Password!);
        dbContext.Users.Add(admin);

        var adminRole = await dbContext.Roles.FirstAsync(x => x.Name!.Equals("Administrator"), token);

        dbContext.UserRoles.Add(new IdentityUserRole<Guid> { RoleId = adminRole.Id, UserId = admin.Id });

        await dbContext.SaveChangesAsync(token);
    }
}
