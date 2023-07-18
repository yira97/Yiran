using System.Net;
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

    public BlogService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var blogAddress = configuration.GetSection(nameof(ServiceSettings) + ":Blog").Get<string>()!;

        _httpClient.BaseAddress = new Uri(blogAddress);
        _httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

    public async Task<PostDto> EditPost(string postId, PostUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Post/{postId}");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PostDto>();
        return result!;
    }

    public async Task<DomainDto> CreateDomain(DomainUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Domain");
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

    public async Task<DomainDto> GetDomainAsync(string domainId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Domain/{domainId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainDto>();
        return result!;
    }

    public async Task<DomainDto> GetDomainByNameAsync(string domainName)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/Domain/nameof/{domainName}");

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
        string? categoryId,
        string? topicId
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

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<CursorBasedQueryResult<PostDto>>();
        return result!;
    }

    public async Task<GetInfo> GetTempGetInfo(string resourceId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/StaticResource/{resourceId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<GetInfo>();
        return result!;
    }

    public async Task<PutInfo> GetPutInfo(StaticResourceUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/StaticResource");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<PutInfo>();
        return result!;
    }

    public async Task<DomainCategoryDto> AddCategory(string domainId, DomainCategoryUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/Domain/{domainId}/categories");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainCategoryDto>();
        return result!;
    }

    public async Task<DomainTopicDto> AddTopic(string domainId, DomainTopicUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/Domain/{domainId}/topics");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainTopicDto>();
        return result!;
    }

    public async Task<DomainCategoryDto> EditCategory(string domainId, string categoryId,
        DomainCategoryUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}/categories/{categoryId}");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainCategoryDto>();
        return result!;
    }

    public async Task<DomainTopicDto> EditTopic(string domainId, string topicId,
        DomainTopicUpdateDto updateDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/Domain/{domainId}/topics/{topicId}");
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<DomainTopicDto>();
        return result!;
    }

    public async Task DeleteCategory(string domainId, string categoryId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}/categories/{categoryId}");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteTopic(string domainId, string topicId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/Domain/{domainId}/topics/{topicId}");

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

    public async Task<SiteMapDto?> GetSiteMap(string domainId, string? language = null)
    {
        var url = $"api/v1/Domain/{domainId}/site-map";
        if (language != null)
        {
            url += $"/{language}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await _httpClient.SendAsync(request);
        if (resp.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        var result = await resp.Content.ReadFromJsonAsync<SiteMapDto>();
        return result!;
    }

    public async Task<SocialLinksDto> GetSocialLinks(string domainId)
    {
        var url = $"api/v1/Domain/{domainId}/social-links";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<SocialLinksDto>();
        return result!;
    }

    public async Task UpdateSocialLinks(string domainId, SocialLinksDto socialLinks)
    {
        var url = $"api/v1/Domain/{domainId}/social-links";
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Content =
            new StringContent(JsonSerializer.Serialize(socialLinks), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task UpdateSiteMap(string domainId, SiteMapDto siteMap)
    {
        var url = $"api/v1/Domain/{domainId}/site-map";
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Content =
            new StringContent(JsonSerializer.Serialize(siteMap), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task UpdateSiteMapTranslation(string domainId, string language, SiteMapDto siteMap)
    {
        var url = $"api/v1/Domain/{domainId}/site-map/translation/{language}";
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Content =
            new StringContent(JsonSerializer.Serialize(siteMap), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<UserInfoDto> GetUserProfile(string userId)
    {
        var url = $"api/v1/Account/profile/{userId}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<UserInfoDto>();
        return result!;
    }

    public async Task<UserInfoDto> GetUserProfile()
    {
        var url = $"api/v1/Account/profile";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<UserInfoDto>();
        return result!;
    }

    public async Task<UserInfoDto> UpdateUserNickName(string nickName)
    {
        var url = $"api/v1/Account/profile/nick-name";
        var request = new HttpRequestMessage(HttpMethod.Patch, url);
        var updateDto = new UpdateUserNickNameDto
        {
            NickName = nickName,
        };
        request.Content =
            new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<UserInfoDto>();
        return result!;
    }
}