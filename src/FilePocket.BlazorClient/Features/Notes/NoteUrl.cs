namespace FilePocket.BlazorClient.Features.Notes
{
    public class NoteUrl
    {
        public static string BaseUrl => $"api/notes";
        public static string GetAllByUserId() => $"{BaseUrl}";
        public static string GetById(Guid id) => $"{BaseUrl}/{id}";
        public static string Create() => $"{BaseUrl}";
        public static string Update(Guid id) => $"{BaseUrl}";
        public static string Delete(Guid id) => $"{BaseUrl}/{id}";
        public static string IrreversiblyDelete(Guid id) => $"{BaseUrl}/{id}/delete-irreversibly";
    }
}