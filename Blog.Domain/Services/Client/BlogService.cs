using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blog.Domain.Enums;
using Blog.Domain.Models;
using Evrane.Core.Helper;
using Evrane.Core.ObjectStorage;
using Evrane.Core.Security;
using Evrane.Core.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Blog.Domain.Services.Client;

public class BlogService
{
    private readonly HttpClient _httpClient;
    private readonly IJwtService _jwtService;

    public BlogService(HttpClient httpClient, IConfiguration configuration, IJwtService jwtService)
    {
        _httpClient = httpClient;
        _jwtService = jwtService;
        var blogAddress = configuration.GetSection(nameof(ServiceSettings) + ":Blog").Get<string>()!;

        _httpClient.BaseAddress = new Uri(blogAddress);
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<(bool, AccessTokenDto)> EnsureAccessToken(AccessTokenDto currentAccessToken)
    {
        var t = _jwtService.GetExpiresTime(currentAccessToken.AccessToken);
        var needRefresh = t < DateTime.UtcNow.AddSeconds(10);
        var newAccessToken = currentAccessToken;
        if (needRefresh)
        {
            newAccessToken = await Refresh(currentAccessToken);
        }

        return (needRefresh, newAccessToken);
    }

    public async Task<AccessTokenDto> EmailRegister(EmailPasswordAuthDto authDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Account/register");
        request.Content = new StringContent(JsonSerializer.Serialize(authDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AccessTokenDto>();
        return result!;
    }

    public async Task<AccessTokenDto> EmailLogin(EmailPasswordAuthDto authDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Account/login");
        request.Content = new StringContent(JsonSerializer.Serialize(authDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AccessTokenDto>();
        return result!;
    }

    public async Task<AccessTokenDto> Refresh(AccessTokenDto accessTokenDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Account/refresh");
        request.Content =
            new StringContent(JsonSerializer.Serialize(accessTokenDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AccessTokenDto>();
        return result!;
    }

    public async Task<List<DomainDto>> ListDomains()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/Domain");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<List<DomainDto>>();
        return result!;
    }

    public async Task<PostDto> CreatePost(PostUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Post");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PostDto>();
        return result!;
    }

    public async Task<DomainDto> CreateDomain(DomainUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Domain");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task DeletePost(string postId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Post/{postId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteDomain(string domainId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<DomainDto> GetDomain(string domainId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Domain/{domainId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task<PostDto> GetPost(string postId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Post/{postId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PostDto>();
        return result!;
    }

    public async Task<CursorBasedQueryResult<PostDto>> ListPosts(
        int pageSize,
        string pageToken,
        int orderBy,
        bool ascending,
        string? domainId,
        bool? publicOnly,
        int? category,
        int? topic
    )
    {
        var query = new Dictionary<string, string>();
        if (domainId != null) query.Add(PostFilterKey.DomainId.IntString(), domainId);
        if (publicOnly != null) query.Add(PostFilterKey.PublicOnly.IntString(), "true");
        if (category != null) query.Add(PostFilterKey.Category.IntString(), category.ToString()!);
        if (topic != null) query.Add(PostFilterKey.Topic.IntString(), topic.ToString()!);

        var requestUri = QueryHelpers.AddQueryString("api/v1/Post", query!);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<CursorBasedQueryResult<PostDto>>();
        return result!;
    }

    public async Task<GetInfo> GetTempGetInfo(string resourceId, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/StaticResource/{resourceId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<GetInfo>();
        return result!;
    }

    public async Task<PutInfo> GetPutInfo(StaticResourceUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/StaticResource");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PutInfo>();
        return result!;
    }
}