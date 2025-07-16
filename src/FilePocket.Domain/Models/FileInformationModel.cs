using Microsoft.AspNetCore.Http;


namespace FilePocket.Domain.Models
{
    public class FileInformationModel
    {
        public Guid PocketId { get; set; }
        public Guid? FolderId { get; set; }
        public IFormFile? File { get; set; }
    }
}
