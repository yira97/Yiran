using System.Security.Cryptography;
using System.Text;
using Evrane.Core.Settings;
using Microsoft.Extensions.Configuration;

namespace Evrane.Core.Security;

public class RsaCryptographyTool : IRsaCryptographyTool
{
    private readonly RsaSettings _rsaSettings;

    public RsaCryptographyTool(IConfiguration configuration)
    {
        _rsaSettings = configuration.GetSection(nameof(RsaSettings)).Get<RsaSettings>()!;

        if (_rsaSettings.Pem != null)
        {
            PublicKeyPem = _rsaSettings.Pem.PublicKey;
            PrivateKeyPem = _rsaSettings.Pem.PrivateKey;
        }
        else if (_rsaSettings.PemFilePath != null)
        {
            PublicKeyPem = File.ReadAllText(_rsaSettings.PemFilePath.PublicKeyFilePath);
            PrivateKeyPem = File.ReadAllText(_rsaSettings.PemFilePath.PrivateKeyFilePath);
        }
    }

    public string PublicKeyPem { get; set; } = string.Empty;
    public string PrivateKeyPem { get; set; } = string.Empty;

    public string Encrypt(string originContent)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(PublicKeyPem);

        // Create a UnicodeEncoder to convert between byte array and string.
        var byteConverter = new UTF8Encoding();
        var originalBytes = byteConverter.GetBytes(originContent);

        var encryptedBytes = rsa.Encrypt(originalBytes, false);
        var encrypted = Convert.ToBase64String(encryptedBytes);
        return encrypted;
    }

    public string Decrypt(string encryptedContent)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(PrivateKeyPem);

        // Create a UnicodeEncoder to convert between byte array and string.
        var encryptedByte = Convert.FromBase64String(encryptedContent);
        var decryptedBytes = rsa.Decrypt(encryptedByte, false);

        var byteConverter = new UTF8Encoding();
        var decrypted = byteConverter.GetString(decryptedBytes);
        return decrypted;
    }
}