namespace FilePocket.Admin.Models.Storage;

public class StorageModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public DateTime DateCreated { get; set; }
    
    public int NumberOfFiles { get; set; }
    
    public double TotalFileSize { get; set; }

    public Guid UserId { get; set; }
}
