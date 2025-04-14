namespace FilePocket.Application.Interfaces.Services
{
    public interface IEncryptionService
    {
        Task<byte[]> EncryptAsync(string clearText, string passPhrase, CancellationToken cancellationToken);
        Task<string> DecryptAsync(byte[] encrypted, string passPhrase, CancellationToken cancellationToken);
        string GeneratePassPhrase(Guid firstId, Guid secondId);
    }
}