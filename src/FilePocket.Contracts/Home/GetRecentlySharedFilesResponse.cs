using FilePocket.Domain;

namespace FilePocket.Contracts.Home
{
    public class GetRecentlySharedFilesResponse
    {
        public Guid SharedFileId { get; set; }
        public FileTypes FileType { get; set; }
        public string OriginalName { get; set; } = string.Empty;
    }
}
