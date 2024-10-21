namespace FilePocket.Admin.Models.Files
{
    public class FileDownloadModel
    {
        public string OriginalName { get; set; } = string.Empty;
        public byte[] FileByteArray { get; set; } = [];
    }
}
