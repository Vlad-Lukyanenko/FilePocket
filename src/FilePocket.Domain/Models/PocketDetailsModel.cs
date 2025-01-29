namespace FilePocket.Domain.Models
{
    public class PocketDetailsModel
    {
        public string? Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfFiles { get; set; }
        public double TotalFileSize { get; set; }
    }
}