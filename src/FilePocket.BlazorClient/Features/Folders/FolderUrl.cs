namespace FilePocket.BlazorClient.Features.Folders
{
    public static class FolderUrl
    {
        public static string BaseUrl => "api";
        public static string Create() => $"{BaseUrl}/folders";
        public static string GetAll(Guid? pocketId, Guid parentFolderId, bool isSoftDeleted, IEnumerable<int> folderTypeValues)
        {
            var folderTypesQueryStringParams = string.Concat("?folderTypes=", string.Join("&folderTypes=", folderTypeValues));
            var url = pocketId is null
                ? $"api/parent-folder/{parentFolderId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}"
                : $"api/pockets/{pocketId}/parent-folder/{parentFolderId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}";

            return url;
        }
        public static string GetAll(Guid? pocketId, bool isSoftDeleted, IEnumerable<int> folderTypeValues)
        {
            var folderTypesQueryStringParams = string.Concat("?folderTypes=", string.Join("&folderTypes=", folderTypeValues));

            var url = pocketId is null
                ? $"api/folders/{isSoftDeleted}{folderTypesQueryStringParams}"
                : $"api/pockets/{pocketId}/{isSoftDeleted}/folders{folderTypesQueryStringParams}";
            return url;
        }
        public static string Get(Guid pocketId, Guid folderId) => $"api/pockets/{pocketId}/folders/{folderId}";
        public static string Delete(Guid folderId) => $"api/folders/{folderId}";
        public static string MoveToTrash(Guid folderId) => $"api/folders/{folderId}";
    }
}
