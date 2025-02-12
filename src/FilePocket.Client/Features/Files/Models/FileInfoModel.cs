using FilePocket.Client.Features.Files.Models;

namespace FilePocket.Client.Services.Files.Models
{
    public class FileInfoModel
    {
        public Guid Id { get; set; }

        public string? OriginalName { get; set; }

        public FileTypes? FileType { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid? PocketId { get; set; }

        public Guid? FolderId { get; set; }

        public double FileSize { get; set; }

        public bool IsSelected { get; set; }

        public bool IsLoaded { get; set; } = true;
        
        public bool HasFailedUpload { get; set; }

        public void MarkAsFailedUpload()
        {
            HasFailedUpload = true;
        }
    }
    
    public class FileUploadError
    {
        public string OriginalName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
