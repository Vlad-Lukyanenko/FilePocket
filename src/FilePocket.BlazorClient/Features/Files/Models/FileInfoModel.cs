using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Services.Files.Models
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
        
        public long UploadedBytes { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage => TotalBytes == 0 ? 0 : (int)((UploadedBytes * 100) / TotalBytes);
    }
    
    public class FileUploadError
    {
        public string OriginalName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
