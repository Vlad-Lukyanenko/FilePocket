using Blazored.LocalStorage;
using FilePocket.Client.Features;
using FilePocket.Client.Features.Authentication;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Services.Authentication.Requests;
using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Services.Folders.Requests;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
            
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<FilePocketApiClient>();

            builder.Services.AddScoped<IAuthentictionRequests, AuthentictionRequests>();
            builder.Services.AddScoped<IPocketRequests, PocketRequests>();
            builder.Services.AddScoped<IFileRequests, FileRequests>();
            builder.Services.AddScoped<IFolderRequests, FolderRequests>();
            builder.Services.AddScoped<IUserRequests, UserRequests>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
