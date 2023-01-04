namespace Evrane.Core.Security;

public interface IRsaCryptographyTool
{
    public string PublicKeyPem { get; }
    public string PrivateKeyPem { get; }
    public string Encrypt(string originContent);
    public string Decrypt(string encryptedContent);
}