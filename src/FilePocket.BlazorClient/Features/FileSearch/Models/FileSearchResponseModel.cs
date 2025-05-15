using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Features.FileSearch.Models
{
    public class FileSearchResponseModel
    {
        public Guid Id { get; set; }

        public string OriginalName { get; set; } = string.Empty;

        public FileTypes FileType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public double FileSize { get; set; }

        public Guid PocketId { get; set; }

        public Guid? FolderId { get; set; }

        public Guid UserId { get; set; }
    }
}
