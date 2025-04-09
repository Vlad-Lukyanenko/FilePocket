namespace FilePocket.Contracts.Pocket;

public class GetPocketResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public Guid UserId { get; set; }
    public int NumberOfFiles { get; set; } = 0;

    public double TotalSize { get; set; } = 0;
}
