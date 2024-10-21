namespace FilePocket.Domain.Models
{
    public class StorageDetailsModel
    {
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfFiles { get; set; }
        public double TotalFileSize { get; set; }
    }
}