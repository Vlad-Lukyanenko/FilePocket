namespace FilePocket.Client.Features.Files
{
    public static class FileUrl
    {
        public static string UploadFile => $"api/files";
        public static string GetFile(Guid fileId) => $"api/files/{fileId}";
        public static string GetFileInfo(Guid fileId) => $"api/files/{fileId}/info";
        public static string GetImageThumbnail(Guid imageId, int size) => $"api/files/{imageId}/thumbnail/{size}";
        public static string GetAll(Guid pocketId, Guid folderId) => $"api/pockets/{pocketId}/folders/{folderId}/files";
        public static string GetAll(Guid pocketId) => $"api/pockets/{pocketId}/files";
        public static string GetAllFromFolder(Guid folderId) => $"api/folders/{folderId}/files";
        public static string GetAll() => $"api/files";
        public static string GetRecentFiles() => $"api/home/recent-files";
        public static string DeleteFile(Guid fileId) => $"api/files/{fileId}";
    }
}
