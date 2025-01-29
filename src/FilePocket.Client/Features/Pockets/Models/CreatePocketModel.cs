namespace FilePocket.Client.Services.Pockets.Models;

public class CreatePocketModel
{
    public string Name { get; set; } = string.Empty;

    public Guid UserId { get; set; }
}
