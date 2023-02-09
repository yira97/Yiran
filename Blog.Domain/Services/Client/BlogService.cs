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

/// <summary>
/// Http Client For Blog Service
/// </summary>
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
        if (string.IsNullOrEmpty(currentAccessToken.AccessToken)) return (false, currentAccessToken);

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

    public async Task<PostDto> CreatePost(PostUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Post");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PostDto>();
        return result!;
    }

    public async Task<PostDto> EditPost(string postId, PostUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Post/{postId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

    public async Task DeletePost(string postId, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Post/{postId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteDomain(string domainId, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<DomainDto> GetDomainAsync(string domainId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Domain/{domainId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task<PostDto> GetPost(string postId, string? accessToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Post/{postId}");
        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
        string? categoryId,
        string? topicId,
        string? accessToken = null
    )
    {
        var query = new Dictionary<string, string>();
        query.Add("pageSize", pageSize.ToString());
        if (!string.IsNullOrEmpty(pageToken)) query.Add("pageToken", pageToken);
        query.Add("orderBy", orderBy.ToString());
        query.Add("ascending", ascending.ToString());
        if (!string.IsNullOrEmpty(domainId)) query.Add("domainId", domainId);
        if (publicOnly != null) query.Add("publicOnly", publicOnly.ToString()!);
        if (!string.IsNullOrEmpty(categoryId)) query.Add("categoryId", categoryId);
        if (!string.IsNullOrEmpty(topicId)) query.Add("topicId", topicId);

        var requestUri = QueryHelpers.AddQueryString("api/v1/Post", query!);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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

    public async Task<DomainCategoryDto> AddCategory(string domainId, DomainCategoryUpdateDto updateDto,
        string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/Domain/{domainId}/categories");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainCategoryDto>();
        return result!;
    }

    public async Task<DomainTopicDto> AddTopic(string domainId, DomainTopicUpdateDto updateDto,
        string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/Domain/{domainId}/topics");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainTopicDto>();
        return result!;
    }

    public async Task<DomainCategoryDto> EditCategory(string domainId, string categoryId,
        DomainCategoryUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}/categories/{categoryId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainCategoryDto>();
        return result!;
    }

    public async Task<DomainTopicDto> EditTopic(string domainId, string topicId,
        DomainTopicUpdateDto updateDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}/topics/{topicId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainTopicDto>();
        return result!;
    }

    public async Task DeleteCategory(string domainId, string categoryId, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}/categories/{categoryId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteTopic(string domainId, string topicId, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}/topics/{topicId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<bool> ExistAdmin()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Account/exist-admin");
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<BoolResultDto>();
        return result!.Result;
    }
}