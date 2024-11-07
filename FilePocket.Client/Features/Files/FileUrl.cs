namespace FilePocket.Client.Features.Files
{
    public static class FileUrl
    {
        public static string UploadFile => $"api/files";
        public static string GetFile(Guid pocketId, Guid fileId) => $"api/pockets/{pocketId}/files/{fileId}";
        public static string GetFileInfo(Guid pocketId, Guid fileId) => $"api/pockets/{pocketId}/files/{fileId}/info";
        public static string GetImageThumbnail(Guid pocketId, Guid imageId, int size) => $"api/pockets/{pocketId}/files/{imageId}/thumbnail/{size}";
        public static string GetAll(Guid pocketId) => $"api/pockets/{pocketId}/files";
        public static string DeleteFile(Guid pocketId, Guid fileId) => $"api/pockets/{pocketId}/files/{fileId}";
    }
}
