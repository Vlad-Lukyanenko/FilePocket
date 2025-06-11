using FilePocket.BlazorClient.Features.Search.Enums;

namespace FilePocket.BlazorClient.Features.Trash
{
    public static class TrashUrl
    {
        public static string MoveFileToTrash(Guid fileId)
        {
            return $"api/trash/files/{fileId}";
        }
        public static string MovePocketToTrash(Guid pocketId)
        {
            return $"api/trash/pockets/{pocketId}";
        }
        public static string ClearAllTrash()
        {
            return "api/trash/clearall";
        }

        public static string GetAllSoftdeleted(RequestedItemType itemType)
        {
            return $"api/trash/{itemType.ToString().ToLower()}";
        }
    }
}
