namespace FilePocket.Client.Services.Pockets
{
    public class PocketUrl
    {
        public static string BaseUrl => "api/pockets";
        public static string GetAll(Guid userId) => $"{BaseUrl}/all/users/{userId}";
        public static string GetInfo(Guid pocketId) => $"{BaseUrl}/{pocketId}/info";
        public static string Update(Guid pocketId) => $"{BaseUrl}/{pocketId}";
        public static string Delete(Guid pocketId) => $"{BaseUrl}/{pocketId}";
    }
}
