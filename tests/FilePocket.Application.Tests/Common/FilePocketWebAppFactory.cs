using FilePocket.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Xunit;

namespace FilePocket.Application.IntegrationTests.Common;

public class FilePocketWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer PostgreSqlContainer { get;  } = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("FilePocket")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // picks up the environment settings from appsettings.Testing.json file from FilePocket.Host project
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            OverridePocketDbContext(services, PostgreSqlContainer.GetConnectionString());
        });
    }

    private static void  OverridePocketDbContext(IServiceCollection services, string containerConnectionString)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FilePocketDbContext>));
        
        if (descriptor is not null) 
            services.Remove(descriptor);

        if (string.IsNullOrWhiteSpace(containerConnectionString))
            throw new ArgumentException("Connection string is null or empty", nameof(containerConnectionString));

        services.AddDbContext<FilePocketDbContext>(options => options.UseNpgsql(containerConnectionString));

        // During migration, we need to use the connection string from the context
        var testConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings:PostgreDb");
        if (string.IsNullOrWhiteSpace(testConnectionString) || testConnectionString != containerConnectionString)
            Environment.SetEnvironmentVariable("ConnectionStrings:PostgreDb", containerConnectionString);
    }

    public Task InitializeAsync()
    {
        return PostgreSqlContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return PostgreSqlContainer.StopAsync();
    }
}