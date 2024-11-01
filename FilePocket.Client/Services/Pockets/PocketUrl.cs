namespace FilePocket.Client.Services.Pockets
{
    public class PocketUrl
    {
        public static string GetAll(Guid userId) => $"api/storages/all/user/{userId}";
    }
}
