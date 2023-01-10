using System.Net;
using Evrane.Core.Exceptions;

namespace Evrane.Core.ObjectStorage.S3;

public class S3HttpClient
{
    private readonly HttpClient _httpClient;

    public S3HttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task PutSignedUrl(string url, Stream file, long fileSize, string contentType)
    {
        var streamContent = new StreamContent(file);
        // include content-length, or get 501
        streamContent.Headers.ContentLength = fileSize;
        streamContent.Headers.Add("Content-Type", contentType);
        var result = await _httpClient.PutAsync(url, streamContent);
        if (result.StatusCode != HttpStatusCode.OK)
            throw new EvraneException(EvraneStatusCode.ServerError, result.ReasonPhrase, "upload failed");
    }
}