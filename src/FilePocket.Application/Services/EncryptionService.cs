using FilePocket.Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace FilePocket.Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _initializationVector =
{
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
    };

        public async Task<byte[]> EncryptAsync(string clearText, string passPhrase, CancellationToken cancellationToken)
        {
            using var aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(passPhrase);
            aes.IV = _initializationVector;

            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await cryptoStream.WriteAsync(Encoding.Unicode.GetBytes(clearText), cancellationToken);
            await cryptoStream.FlushFinalBlockAsync(cancellationToken);

            return output.ToArray();
        }

        public async Task<string> DecryptAsync(byte[] encrypted, string passPhrase, CancellationToken cancellationToken)
        {
            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(passPhrase);
            aes.IV = _initializationVector;

            using MemoryStream input = new(encrypted);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output, cancellationToken);

            return Encoding.Unicode.GetString(output.ToArray());
        }

        public string GeneratePassPhrase(Guid firstId, Guid secondId)
        {
            var firstString = firstId.ToString();
            var secondString = secondId.ToString();

            var passPhraseBlank = firstString.CompareTo(secondString) > 0
                                    ? string.Concat(firstString, secondString)
                                    : string.Concat(secondString, firstString);

            using MD5 md5Hash = MD5.Create();

            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(passPhraseBlank));
            var sBuilder = new StringBuilder();

            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private byte[] DeriveKeyFromPassword(string password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16; // 16 bytes equal 128 bits.
            var hashMethod = HashAlgorithmName.SHA384;

            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }
    }
}
