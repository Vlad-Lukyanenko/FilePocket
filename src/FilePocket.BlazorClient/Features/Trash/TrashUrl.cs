using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Features.Trash
{
    public static class TrashUrl
    {
        public static string RestoreFromTrash(string itemType, string itemId)
        {
            return $"api/trash/{itemType.ToLower()}/{itemId}/restore";
        }
        public static string ClearAllTrash()
        {
            return "api/trash/clearall";
        }
        public static string GetAllDeletedItems(RequestedItemType itemType)
        {
            return $"api/trash/{itemType.ToString().ToLower()}";
        }
        public static string GetDeletedItem(string itemType, string itemId)
        {
            return $"api/trash/{itemType.ToLower()}/{itemId}";
        }
    }
}
