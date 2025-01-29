using FilePocket.Client.Features.Files.Models;

namespace FilePocket.Client.Services.Files.Models
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

        public DateTime DateCreated { get; set; }
    }
}
