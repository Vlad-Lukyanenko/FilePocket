﻿using BirdMessenger;
using Blazored.LocalStorage;
using FilePocket.BlazorClient;
using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Authentication;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Features.Trash;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TusDotNetClient;
using TusClient = BirdMessenger.TusClient;

namespace FilePocket.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var apiUrl = builder.Configuration.GetValue<string>("BaseAddresses:ApiBaseUrl")!;

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

            builder.Services.AddHttpClient<FilePocketApiClient>("FilePocketApi", client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // builder.Services.AddScoped<FilePocketDotNetTusClient>();
            builder.Services.AddHttpClient<ITusClient, TusClient>();
            builder.Services.AddScoped<FilePocketBirdClient>();
            // builder.Services.AddScoped<TusUploadService>();

            builder.Services.AddScoped<NavigationHistoryService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<FilePocketApiClient>();

            builder.Services.AddScoped<IAuthentictionRequests, AuthentictionRequests>();
            builder.Services.AddScoped<IPocketRequests, PocketRequests>();
            builder.Services.AddScoped<IFileRequests, FileRequests>();
            builder.Services.AddScoped<IFolderRequests, FolderRequests>();
            builder.Services.AddScoped<IUserRequests, UserRequests>();
            builder.Services.AddScoped<ISharedFilesRequests, SharedFilesRequests>();
            builder.Services.AddScoped<ITrashRequests, TrashRequests>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
