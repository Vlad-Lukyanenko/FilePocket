namespace FilePocket.BlazorClient.Features.Files.Models
{
    public class RecentlyUploadedFileDto
    {
        public Guid Id { get; set; }
        public Guid PocketId { get; set; }
        public Guid? FolderId { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public FileTypes FileType { get; set; }
        public bool IsLoaded { get; set; }
    }
}
