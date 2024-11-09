using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Services.Folders.Requests;
using FilePocket.Client.Services.Pockets.Requests;
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
            builder.Services.AddHttpClient("FilePocketApi", client =>
                client.BaseAddress = new Uri(apiUrl));

            builder.Services.AddSingleton<IPocketRequests, PocketRequests>();
            builder.Services.AddSingleton<IFileRequests, FileRequests>();
            builder.Services.AddSingleton<IFolderRequests, FolderRequests>();


            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
