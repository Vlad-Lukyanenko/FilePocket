using FilePocket.Admin.AuthFeatures;
using FilePocket.Admin.Components;
using FilePocket.Admin.Requests;
using FilePocket.Admin.Requests.Contracts;
using FilePocket.Admin.Requests.HttpRequests;
using FilePocket.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRadzenComponents();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient("AuthApi", client =>
    client.BaseAddress = new Uri(builder.Configuration["BaseAddresses:ApiBaseUrl"]!));

builder.Services.AddHttpClient("FilePocketApi", client =>
    client.BaseAddress = new Uri(builder.Configuration["BaseAddresses:ApiBaseUrl"]!));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IAuthentictionRequests, AuthentictionRequests>();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<QueryStringConverter>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IStorageRequests, StorageRequests>();
builder.Services.AddScoped<IFileRequests, FileRequests>();

builder.Services.AddScoped<IUserRequests, UserRequests>();
builder.Services.AddScoped<IHttpRequests, HttpAuthorizedRequests>(
    serviceProvider => new HttpAuthorizedRequests(
        factory: serviceProvider.GetRequiredService<IHttpClientFactory>(),
        protectedLocalStorage: serviceProvider.GetRequiredService<ProtectedLocalStorage>(),
        navigationManager: serviceProvider.GetRequiredService<NavigationManager>(),
        authRequests: serviceProvider.GetRequiredService<IAuthentictionRequests>(),
        httpClientName: "FilePocketApi")
    );
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
