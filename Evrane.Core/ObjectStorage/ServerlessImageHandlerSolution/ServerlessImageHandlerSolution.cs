using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Evrane.Core.Settings;

namespace Evrane.Core.ObjectStorage.ServerlessImageHandlerSolution;

public class ServerlessImageHandlerSolution : S3.S3
{
    private ServerlessImageHandlerSolutionConfiguration _solutionConfiguration =>
        _awsSettings.AwsSolutionSettings.ServerlessImageHandlerSolutionConfiguration;

    private readonly IAmazonSecretsManager _secretsManager;
    private string? _secretValue;

    private async Task<string> GetSecretValue()
    {
        if (_secretValue != null) return _secretValue;
        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = _solutionConfiguration.SecretsManagerName,
        };
        GetSecretValueResponse response;

        try
        {
            response = await _secretsManager.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            // For a list of the exceptions thrown, see
            // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
            throw e;
        }

        var secret = response.SecretString;
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
        _secretValue = keyValuePairs?[_solutionConfiguration.SecretKey];
        return _secretValue!;
    }

    public ServerlessImageHandlerSolution(IConfiguration configuration) : base(configuration)
    {
        var awsCredentials = new BasicAWSCredentials(_awsSettings.AccessKeyId, _awsSettings.SecretAccessKey);

        _secretsManager =
            new AmazonSecretsManagerClient(awsCredentials, RegionEndpoint.GetBySystemName(_awsSettings.Region));
    }

    public override async Task<ImageGetInfoDto> ImageGetInfo(string key, TimeSpan duration,
        Dictionary<int, string>? extraInfo = null)
    {
        var secret = await GetSecretValue();
        var bSecret = Encoding.ASCII.GetBytes(secret);

        key = key.StartsWith("/") ? key : $"/{key}";

        var xs = $"/fit-in/100x100/{key}";
        var sm = $"/filters:quality(80)/fit-in/400x400/{key}";
        var md = $"/filters:quality(80)/fit-in/1000x1000/{key}";
        var lg = $"/filters:quality(75)/fit-in/2000x2000/{key}";
        var xl = $"/filters:quality(70)/fit-in/3000x3000/{key}";

        using HMACSHA256 hmac = new HMACSHA256(bSecret);

        byte[] bKey;
        byte[] hashValue;
        string hash;

        bKey = Encoding.ASCII.GetBytes(xs);
        hashValue = hmac.ComputeHash(bKey);
        hash = Convert.ToHexString(hashValue).ToLower();
        var resultUrlXs = $"https://{_solutionConfiguration.DistributionDomainName}{xs}?signature={hash}";

        bKey = Encoding.ASCII.GetBytes(sm);
        hashValue = hmac.ComputeHash(bKey);
        hash = Convert.ToHexString(hashValue).ToLower();
        var resultUrlSm = $"https://{_solutionConfiguration.DistributionDomainName}{sm}?signature={hash}";

        bKey = Encoding.ASCII.GetBytes(md);
        hashValue = hmac.ComputeHash(bKey);
        hash = Convert.ToHexString(hashValue).ToLower();
        var resultUrlMd = $"https://{_solutionConfiguration.DistributionDomainName}{md}?signature={hash}";

        bKey = Encoding.ASCII.GetBytes(lg);
        hashValue = hmac.ComputeHash(bKey);
        hash = Convert.ToHexString(hashValue).ToLower();
        var resultUrlLg = $"https://{_solutionConfiguration.DistributionDomainName}{lg}?signature={hash}";

        bKey = Encoding.ASCII.GetBytes(xl);
        hashValue = hmac.ComputeHash(bKey);
        hash = Convert.ToHexString(hashValue).ToLower();
        var resultUrlXl = $"https://{_solutionConfiguration.DistributionDomainName}{xl}?signature={hash}";

        var resultExpiresAt = DateTime.Now.AddDays(1);

        return new ImageGetInfoDto(Url: resultUrlLg, UrlXs: resultUrlXs, UrlSm: resultUrlSm, UrlMd: resultUrlMd,
            UrlLg: resultUrlLg, UrlXl: resultUrlXl, ExpiresAt: resultExpiresAt, "");
    }
}