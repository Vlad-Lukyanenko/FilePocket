using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Services.Files.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public Guid? PocketId { get; set; }

        public string? OriginalName { get; set; }

        public byte[]? FileByteArray { get; set; }

        public string? FileData { get; set; }

        public FileTypes? FileType { get; set; }

        public double FileSize { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
