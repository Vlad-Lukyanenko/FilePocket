namespace FilePocket.Domain.Models
{
    public class FileSearchResponseModel : SearchResponseModel
    {
        public string OriginalName { get; set; } = string.Empty;
        public FileTypes FileType { get; set; }
        public double FileSize { get; set; }
    }
}
