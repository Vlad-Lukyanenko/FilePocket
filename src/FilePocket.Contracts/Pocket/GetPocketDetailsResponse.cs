namespace FilePocket.Contracts.Pocket;

public class GetPocketDetailsResponse
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public int NumberOfFiles { get; set; }
    public double TotalFileSize { get; set; }
}
