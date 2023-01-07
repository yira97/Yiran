using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Blog.Domain.Models;
using Evrane.Core.Settings;
using Microsoft.Extensions.Configuration;

namespace Blog.Domain.Services.Client;

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

    public async Task<AccessTokenDto> Refresh(AccessTokenDto accessTokenDto)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/Account/register");
        request.Content =
            new StringContent(JsonSerializer.Serialize(accessTokenDto), Encoding.UTF8, "application/json");

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();
        var result = await resp.Content.ReadFromJsonAsync<AccessTokenDto>();
        return result!;
    }
}