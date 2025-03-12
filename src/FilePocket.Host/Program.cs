using FilePocket.WebApi;
using FilePocket.Shared.Extensions;
using Serilog;
using FilePocket.Host;
using FilePocket.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FilePocket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FilePocket.Domain.Models;
using Microsoft.OpenApi.Models;
using FilePocket.Domain.Models.Configuration;
using FilePocket.WebApi.Attributes;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using FilePocket.Application.Interfaces.Services;
using FilePocket.Application.Interfaces.Repositories;
using FastEndpoints;
using Mapster;
using FilePocket.Infrastructure.Persistence.Repositories;
using FilePocket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

builder.Services.AddDbContext<FilePocketDbContext>(options =>
{
    options.UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("PostgreDb"));
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(WebApiAssemblyReference).Assembly);

builder.Services.AddFastEndpoints();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {

        builder.WithOrigins(allowedOrigins!) // Allow the frontend origin
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials() // If using cookies or authentication
               .WithExposedHeaders("X-Pagination");
    });
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 1073741824; // 1 GB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // 1 GB
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddIdentity<User, Role>(o =>
{
    o.Password.RequireDigit = true;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 6;
    o.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<FilePocketDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.Configure<AdminSeedingDataModel>(builder.Configuration.GetSection("AdminSeedingData"));
builder.Services.Configure<AccountConsumptionConfigurationModel>(builder.Configuration.GetSection(key: AccountConsumptionConfigurationModel.Section));
builder.Services.Configure<JwtConfigurationModel>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<ApiKeyConfigurationModel>(builder.Configuration.GetSection("ApiKeySettings"));
builder.Services.AddHostedService<InitialRolesAndAdminSeeding>();

// Add services to the container.
builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(WebApiAssemblyReference));
builder.Services.AddMapster();

builder.Services.AddSwaggerGen(s =>
{
    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Place to add JWT with Bearer",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    s.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"  },
                Name = "Bearer",
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddSingleton<IUploadService, UploadService>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<JwtOrApiKeyAuthorizeAttribute>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseFastEndpoints();

app.Run();


// Used by FilePocket.Application.IntegrationTests project
public partial class Program {}
