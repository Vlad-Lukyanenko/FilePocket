using FilePocket.BlazorClient.Features.Files.Models;

namespace FilePocket.BlazorClient.Features.Search.Models
{
    public class FileSearchResponseModel : SearchResponseModel
    {
        public string OriginalName { get; set; } = string.Empty;
        public FileTypes FileType { get; set; }
        public double FileSize { get; set; }
    }
}
