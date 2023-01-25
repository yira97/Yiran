using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Evrane.Core.Settings;
using Microsoft.Extensions.Configuration;

namespace Evrane.Core.ObjectStorage.S3;

public class S3 : IObjectStorageService
{
    protected readonly AwsSettings _awsSettings;
    protected S3Settings S3Settings => _awsSettings.S3Settings;
    protected readonly IAmazonS3 _client;

    public S3(IConfiguration configuration)
    {
        _awsSettings = configuration.GetSection(nameof(AwsSettings)).Get<AwsSettings>()!;

        var awsCredentials = new BasicAWSCredentials(_awsSettings.AccessKeyId, _awsSettings.SecretAccessKey);

        _client = new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_awsSettings.Region));
    }

    public Task<PutInfo> PutInfo(string key, TimeSpan duration, Dictionary<int, string>? extraInfo)
    {
        var expiresAt = DateTime.UtcNow.Add(duration);
        // Create a CopyObject request
        var request = new GetPreSignedUrlRequest
        {
            BucketName = S3Settings.BucketName,
            Key = key,
            Expires = expiresAt,
            Verb = HttpVerb.PUT,
            Protocol = Protocol.HTTPS,
        };

        var url = _client.GetPreSignedURL(request);

        return Task.FromResult(new PutInfo
            (Url: url, ExpiresAt: expiresAt.ToUniversalTime(), ""));
    }

    public Task<GetInfo> GetInfo(string key, TimeSpan duration, Dictionary<int, string>? extraInfo = null)
    {
        var expiresAt = DateTime.UtcNow.Add(duration);
        // Create a CopyObject request
        var request = new GetPreSignedUrlRequest
        {
            BucketName = S3Settings.BucketName,
            Key = key,
            Expires = expiresAt,
        };

        // Get path for request
        var url = _client.GetPreSignedURL(request);

        return Task.FromResult(new GetInfo
        (
            url,
            expiresAt.ToUniversalTime()
            , ""
        ));
    }

    public virtual async Task<ImageGetInfoDto> ImageGetInfo(string key, TimeSpan duration,
        Dictionary<int, string>? extraInfo = null)
    {
        var result = await GetInfo(key, duration);

        return new ImageGetInfoDto
        (
            result.Url,
            result.Url,
            result.Url,
            result.Url,
            result.Url,
            result.Url,
            result.ExpiresAt,
            ""
        );
    }
}