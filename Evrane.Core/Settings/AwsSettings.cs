namespace Evrane.Core.Settings;

public class AwsSettings
{
    public string AccessKeyId { get; set; } = string.Empty;

    public string SecretAccessKey { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public S3Settings S3Settings { get; set; } = new S3Settings();

    public AwsSolutionSettings AwsSolutionSettings  { get; set; }= new AwsSolutionSettings();
}