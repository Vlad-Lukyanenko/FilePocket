namespace FilePocket.BlazorClient.Features.Notes
{
    public class NoteUrl
    {
        public static string BaseUrl => "api";
        public static string GetAllByUserIdAndFolderId(Guid? folderId) => 
            folderId==null
            ? $"{BaseUrl}/notes"
            : $"{BaseUrl}/folders/{folderId}/notes";
        public static string GetById(Guid id) => $"{BaseUrl}/notes/{id}";
        public static string Create() => $"{BaseUrl}/notes";
        public static string Update() => $"{BaseUrl}/notes";
        public static string MoveToTrash(Guid id) => $"{BaseUrl}/notes/{id}";
        public static string Delete(Guid id) => $"{BaseUrl}/notes/{id}";
    }
}