using System.Net.NetworkInformation;

namespace FilePocket.BlazorClient.Features.Storage
{
    public class StorageUrl
    {
        public static string BaseUrl => "api/storage";
        public static string GetStorageConsumption() => $"{BaseUrl}/storageConsumption";
    }
}
