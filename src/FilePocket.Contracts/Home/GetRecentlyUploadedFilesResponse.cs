using FilePocket.Domain;

namespace FilePocket.Contracts.Home
{
    public class GetRecentlyUploadedFilesResponse
    {
        public Guid Id { get; set; }
        public Guid PocketId { get; set; }
        public Guid? FolderId { get; set; }
        public string OriginalName { get; set; } = string.Empty;
        public FileTypes FileType { get; set; }
    }
}
