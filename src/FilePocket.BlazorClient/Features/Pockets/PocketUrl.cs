namespace FilePocket.BlazorClient.Services.Pockets
{
    public class PocketUrl
    {
        public static string BaseUrl => "api/pockets";
        public static string GetAllCustom() => $"{BaseUrl}/all";
        public static string GetDefault() => $"{BaseUrl}/default";
        public static string GetInfo(Guid pocketId) => $"{BaseUrl}/{pocketId}/info";
        public static string Update(Guid pocketId) => $"{BaseUrl}/{pocketId}";
        public static string Delete(Guid pocketId) => $"{BaseUrl}/{pocketId}";
    }
}
