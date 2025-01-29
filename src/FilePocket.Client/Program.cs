using Blazored.LocalStorage;
using FilePocket.Client.Features;
using FilePocket.Client.Features.Authentication;
using FilePocket.Client.Features.SharedFiles.Requests;
using FilePocket.Client.Features.Users.Requests;
using FilePocket.Client.Services.Authentication.Requests;
using FilePocket.Client.Services.Files.Requests;
using FilePocket.Client.Services.Folders.Requests;
using FilePocket.Client.Services.Pockets.Requests;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tavenem.Blazor.IndexedDB;
using Tavenem.DataStorage;

namespace FilePocket.Client
{
    //[JsonSerializable(typeof(IIdItem))]
    //[JsonSerializable(typeof(Item))]
    //public partial class ItemContext : JsonSerializerContext;

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

            //var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            //options.TypeInfoResolverChain.Add(ItemContext.Default.WithAddedModifier(static typeInfo =>
            //{
            //    if (typeInfo.Type == typeof(IIdItem))
            //    {
            //        typeInfo.PolymorphismOptions ??= new JsonPolymorphismOptions
            //        {
            //            IgnoreUnrecognizedTypeDiscriminators = true,
            //            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
            //        };
            //        typeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Item), Item.ItemTypeName));
            //    }
            //}));

            //builder.Services.AddIndexedDbService();
            //builder.Services.AddIndexedDb(
            //    "Tavenem.Blazor.IndexedDB.Sample",
            //    ["SampleStore"],
            //    1,
            //    options);

            //builder.Services.AddIndexedDB(options =>
            //{
            //    options.DbName = "FilePocketCache";
            //    options.Version = 1;

            //    options.Stores.Add(new StoreSchema
            //    {
            //        Name = "thumbnails",
            //        PrimaryKey = new IndexSpec { Name = "fileName", KeyPath = "fileName", Auto = false },
            //        Indexes = new List<IndexSpec>
            //        {
            //            new IndexSpec { Name = "fileName", KeyPath = "fileName", Auto = false }
            //        }
            //    });
            //});
            //builder.Services.AddScoped<ThumbnailCacheService>();

            builder.Services.AddScoped<NavigationHistoryService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<FilePocketApiClient>();

            builder.Services.AddScoped<IAuthentictionRequests, AuthentictionRequests>();
            builder.Services.AddScoped<IPocketRequests, PocketRequests>();
            builder.Services.AddScoped<IFileRequests, FileRequests>();
            builder.Services.AddScoped<IFolderRequests, FolderRequests>();
            builder.Services.AddScoped<IUserRequests, UserRequests>();
            builder.Services.AddScoped<ISharedFilesRequests, SharedFilesRequests>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();

            await builder.Build().RunAsync();
        }
    }
}
