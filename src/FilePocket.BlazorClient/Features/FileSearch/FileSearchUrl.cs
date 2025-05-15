namespace FilePocket.BlazorClient.Features.FileSearch;

public static class FileSearchUrl
{
    public static string GetFilesByPartialName(string partialName) => $"api/file-search/{partialName}";
}
