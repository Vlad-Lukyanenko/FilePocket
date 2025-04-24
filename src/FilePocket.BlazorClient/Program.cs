using Blazored.LocalStorage;
using FilePocket.BlazorClient;
using FilePocket.BlazorClient.Features;
using FilePocket.BlazorClient.Features.Authentication;
using FilePocket.BlazorClient.Features.Bookmarks.Requests;
using FilePocket.BlazorClient.Features.Notes.Requests;
using FilePocket.BlazorClient.Features.Profiles.Models;
using FilePocket.BlazorClient.Features.Profiles.Requests;
using FilePocket.BlazorClient.Features.SharedFiles.Requests;
using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Features.Storage.Requests;
using FilePocket.BlazorClient.Features.Trash;
using FilePocket.BlazorClient.Features.Users.Models;
using FilePocket.BlazorClient.Features.Users.Requests;
using FilePocket.BlazorClient.Helpers;
using FilePocket.BlazorClient.Services.Authentication.Requests;
using FilePocket.BlazorClient.Services.Files.Requests;
using FilePocket.BlazorClient.Services.Folders.Requests;
using FilePocket.BlazorClient.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace FilePocket.Client;

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
        builder.Services.AddScoped<IBookmarkRequests, BookmarkRequests>();
        builder.Services.AddScoped<IStorageRequests, StorageRequests> ();
        builder.Services.AddScoped<IProfileRequests, ProfileRequests>();
        builder.Services.AddScoped<INoteRequests, NoteRequests>();

        builder.Services.AddSingleton<StateContainer<LoggedInUserModel>>();
        builder.Services.AddSingleton<StateContainer<StorageConsumptionModel>>();
        builder.Services.AddSingleton<AppState>();

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore();

        await builder.Build().RunAsync();
    }
}
