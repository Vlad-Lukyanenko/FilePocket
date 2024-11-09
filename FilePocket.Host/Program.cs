using FilePocket.WebApi;
using FilePocket.Shared.Extensions;
using Serilog;
using FilePocket.Host;
using FilePocket.Application.Services;
using FilePocket.Contracts.Services;
using FilePocket.DataAccess.Repositories;
using FilePocket.DataAccess;
using FilePocket.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FilePocket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using FilePocket.Domain.Models;
using Microsoft.OpenApi.Models;

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

var frontendUrl = builder.Configuration["FrontendUrl"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {

        builder.WithOrigins(frontendUrl!) // Allow the frontend origin
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials() // If using cookies or authentication
               .WithExposedHeaders("X-Pagination");
    });
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
builder.Services.Configure<JwtConfigurationModel>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddHostedService<InitialRolesAndAdminSeeding>();

// Add services to the container.
builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(WebApiAssemblyReference));
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

var app = builder.Build();
app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyMigrations();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
