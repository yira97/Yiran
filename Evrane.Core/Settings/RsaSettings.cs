namespace Evrane.Core.Settings;

public class Pem
{
    public string PublicKey { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}

public class PemFilePath
{
    public string PublicKeyFilePath { get; set; } = string.Empty;
    public string PrivateKeyFilePath { get; set; } = string.Empty;
}

public class RsaSettings
{
    public Pem? Pem { get; set; }

    public PemFilePath? PemFilePath { get; set; }
}