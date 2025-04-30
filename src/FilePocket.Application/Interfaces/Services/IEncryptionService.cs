namespace FilePocket.Application.Interfaces.Services
{
    public interface IEncryptionService
    {
        Task<byte[]> EncryptAsync(string clearText, string passPhrase, CancellationToken cancellationToken);
        Task<string> DecryptAsync(byte[] encrypted, string passPhrase, CancellationToken cancellationToken);
        string GeneratePassPhrase(Guid key1, Guid key2);
        Task<byte[]> EncryptContent(Guid key1, Guid key2, string text, CancellationToken cancellationToken = default);
        Task<string> DecryptContent(Guid key1, Guid key2, byte[] encryptedContent, CancellationToken cancellationToken = default);
    }
}