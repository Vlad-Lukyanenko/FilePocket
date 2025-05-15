namespace FilePocket.BlazorClient.Features.Files;

public static class FileUrl
{
    public static string UploadFile => $"api/files";
    public static string GetFile(Guid fileId) => $"api/files/{fileId}";
    public static string GetFileInfo(Guid fileId) => $"api/files/{fileId}/info";
    public static string GetImageThumbnail(Guid imageId, int size) => $"api/files/{imageId}/thumbnail/{size}";
    public static string GetAll(Guid pocketId, Guid folderId, bool isSoftDeleted) => $"api/pockets/{pocketId}/folders/{folderId}/{isSoftDeleted}/files";
    public static string GetAll(Guid pocketId, bool isSoftDeleted) => $"api/pockets/{pocketId}/{isSoftDeleted}/files";
    public static string GetAllFromFolder(Guid folderId) => $"api/folders/{folderId}/files";
    public static string GetAll() => $"api/files";
    public static string Update() => "api/files";
    public static string GetRecentFiles() => $"api/home/files/recent";
    public static string DeleteFile(Guid fileId) => $"api/files/{fileId}";
    public static string GetAllFilesWithSoftDeleted(Guid pocketId) => $"api/pockets/{pocketId}/files";
}
