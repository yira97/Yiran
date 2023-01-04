using System.Globalization;
using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Api.Entities.Nested;
using Blog.Domain.Enums;
using Blog.Api.Helper;
using Blog.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostRepository> _logger;

    public PostRepository(ApplicationDbContext context, ILogger<PostRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 不检查 Domain 是否存在
    /// </summary>
    /// <param name="updateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    public StaticResourceRelatedResult<PostDto> CreatePost(PostUpdateDto updateDto, string userId)
    {
        var model = new PostEntity();

        var now = DateTime.UtcNow;

        model.Title = updateDto.Title;
        model.SubTitle = updateDto.SubTitle;
        model.Slug = updateDto.Slug;
        model.Topic = updateDto.Topic;
        model.Category = updateDto.Category;
        model.Language = updateDto.Language;
        model.isPublic = updateDto.IsPublic;

        model.Content = updateDto.Content.GeneratePostContent();

        // 统计新增资源
        var newElements = model.PostDto().CollectStaticResource().ToList();

        model.DomainId = updateDto.DomainId!;

        model.CreatedById = userId;
        model.CreatedAt = now;
        model.UpdatedById = userId;
        model.UpdatedAt = now;

        _context.Add(model);

        var result = new StaticResourceRelatedResult<PostDto>()
        {
            Data = model.PostDto(),
            Added = newElements
        };

        return result;
    }


    public async Task<CursorBasedQueryResult<PostDto>> ListPosts(CursorBasedQuery order)
    {
        var query = _context.Posts.Where(p => p.DeletedAt == null);

        // 排序
        switch ((PostOrderBy)order.OrderBy)
        {
            case PostOrderBy.CreatedAt:
                query = order.Ascending ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt);

                DateTime? createdAtOrdercreatedAt = null;
                var createdAtOrdercreatedAtKey = nameof(PostOrderBy.CreatedAt);

                try
                {
                    if (!string.IsNullOrEmpty(order.PageToken))
                    {
                        var m = PageTokenHelper.ToDict(order.PageToken);
                        createdAtOrdercreatedAt = DateTime.Parse(m[createdAtOrdercreatedAtKey]);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation("parse page token failed: pageToken = {PageToken}, detail = {Detail}",
                        order.PageToken, e.ToString());
                }

                if (createdAtOrdercreatedAt != null)
                {
                    query = query.Where(e =>
                        order.Ascending
                            ? e.CreatedAt >= createdAtOrdercreatedAt
                            : e.CreatedAt <= createdAtOrdercreatedAt
                    );
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 筛选
        foreach (var (key, value) in order.Filter)
        {
            var filterKey = (PostFilterKey)key;
            var count = value.Count;

            switch (filterKey)
            {
                case PostFilterKey.DomainId:
                    // 长度范围： 等于1
                    if (count != 1) throw new BadHttpRequestException("length invalid");
                    query = query.Where(p => p.DomainId == value[0]);
                    break;
                case PostFilterKey.PublicOnly:
                    // 长度范围： 等于1
                    if (count != 1) throw new BadHttpRequestException("length invalid");
                    query = query.Where(p => p.isPublic == true);
                    break;
                case PostFilterKey.Topic:
                    // 长度范围： 等于1
                    if (count != 1) throw new BadHttpRequestException("length invalid");
                    if (!int.TryParse(value[0], out var topic))
                        throw new BadHttpRequestException("topic should be a integer");
                    query = query.Where(p => p.Topic == topic);
                    break;
                case PostFilterKey.Category:
                    // 长度范围： 等于1
                    if (count != 1) throw new BadHttpRequestException("length invalid");
                    if (!int.TryParse(value[0], out var category))
                        throw new BadHttpRequestException("category should be a integer");
                    query = query.Where(p => p.Category == category);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // 个数
        query = query.Take(order.PageSize + 1);
        _logger.LogInformation("Exec sql: {Sql}", query.ToQueryString());

        var queryResult = await query.ToListAsync();
        var returnList = queryResult.Take(order.PageSize).Select(e => e.PostDto()).ToList();

        var result = new CursorBasedQueryResult<PostDto>
        {
            Data = returnList,
            HasMore = queryResult.Count > order.PageSize,
            PreviousPage = order.PageToken,
        };

        if (result.HasMore)
        {
            var next = queryResult.Last();
            result.NextPage = (PostOrderBy)order.OrderBy switch
            {
                PostOrderBy.CreatedAt => PageTokenHelper.FromDict(new Dictionary<string, string>
                {
                    {
                        nameof(PostOrderBy.CreatedAt),
                        next.CreatedAt.ToUniversalTime().ToString(CultureInfo.InvariantCulture)
                    },
                }),
            };
        }

        return result;
    }

    public async Task<IEnumerable<DomainDto>> ListAllDomains()
    {
        var query = _context.Domains.Take(10).AsQueryable();

        var result = await query.ToListAsync();

        return result.Select(r => r.DomainDto()).ToList();
    }

    public DomainDto CreateDomain(DomainUpdateDto updateDto, string userId)
    {
        var model = new DomainEntity();

        model.Name = updateDto.Name;

        model.UpdatedById = userId;
        model.CreatedById = userId;

        _context.Add(model);

        return model.DomainDto();
    }

    private async Task<DomainEntity?> GetDomainOrDefault(string domainId)
    {
        var domain = await _context.Domains.FindAsync(domainId);

        if (domain == null || domain.DeletedAt != null)
        {
            return null;
        }

        return domain;
    }

    private async Task<DomainEntity> GetDomain(string domainId)
    {
        var domain = await _context.Domains.FindAsync(domainId);

        if (domain == null || domain.DeletedAt != null)
        {
            throw new BadHttpRequestException("id not found");
        }

        return domain;
    }

    public async Task<DomainDto?> GetDomainInfo(string domainId)
    {
        var domain = await GetDomainOrDefault(domainId);
        return domain?.DomainDto();
    }

    public async Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto, string userId)
    {
        var domain = await GetDomain(domainId);

        domain.Name = updateDto.Name;

        domain.UpdatedAt = DateTime.UtcNow;
        domain.UpdatedById = userId;

        return domain.DomainDto();
    }

    public async Task<bool> DeleteDomain(string domainId, string userId)
    {
        var domain = await _context.Domains.FindAsync(domainId);
        if (domain == null || domain.DeletedAt == null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        domain.UpdatedById = userId;
        domain.UpdatedAt = now;
        domain.DeletedAt = now;

        _logger.LogInformation("delete domain");

        return true;
    }

    private async Task<PostEntity?> GetPostOrDefault(string postId)
    {
        var post = await _context.Posts.FindAsync(postId);

        if (post == null || post.DeletedAt != null)
        {
            return null;
        }

        return post;
    }

    private async Task<PostEntity> GetPost(string postId)
    {
        var post = await _context.Posts.FindAsync(postId);

        if (post == null || post.DeletedAt != null)
        {
            throw new BadHttpRequestException("id not found");
        }

        return post;
    }

    public async Task<PostDto?> GetPostInfo(string postId)
    {
        var post = await GetPostOrDefault(postId);

        return post?.PostDto();
    }

    public async Task<StaticResourceRelatedResult<PostDto>> UpdatePost(string postId, PostUpdateDto updateDto,
        string userId)
    {
        var post = await GetPost(postId);
        var current = post.PostDto();

        var result = new StaticResourceRelatedResult<PostDto>();

        post.Title = updateDto.Title;
        post.SubTitle = updateDto.SubTitle;
        post.Slug = updateDto.Slug;
        post.Topic = updateDto.Topic;
        post.Category = updateDto.Category;
        post.Language = updateDto.Language;
        post.isPublic = updateDto.IsPublic;
        post.Content = updateDto.Content.GeneratePostContent();

        post.UpdatedAt = DateTime.UtcNow;
        post.UpdatedById = userId;

        var newState = post.PostDto();

        var diff = current.CompareStaticResource(newState);
        if (diff.Data)
        {
            result.Added = diff.Added;
            result.Removed = diff.Removed;
        }

        result.Data = newState;

        return result;
    }

    public async Task<StaticResourceRelatedResult<bool>> DeletePost(string postId, string userId)
    {
        var post = await GetPostOrDefault(postId);

        var result = new StaticResourceRelatedResult<bool>();

        if (post == null)
        {
            result.Data = false;
            return result;
        }

        var now = DateTime.UtcNow;
        post.UpdatedById = userId;
        post.UpdatedAt = now;
        post.DeletedAt = now;

        _logger.LogInformation("delete post");

        result.Data = true;
        result.Removed = post.PostDto().CollectStaticResource().Select(e => e.ResourceId).ToList();

        return result;
    }
}