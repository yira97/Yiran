using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IPostRepository
{
    /// <summary>
    /// 不检查 Domain 是否存在
    /// </summary>
    /// <param name="updateDto"></param>
    /// <param name="domainId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    StaticResourceRelatedResult<PostDto> CreatePost(PostUpdateDto updateDto, string userId);

    Task<PostDto?> GetPostInfo(string postId);

    Task<CursorBasedQueryResult<PostDto>> ListPosts(CursorBasedQuery order);
    Task<IEnumerable<DomainDto>> ListAllDomains();
    DomainDto CreateDomain(DomainUpdateDto updateDto, string userId);
    Task<DomainDto?> GetDomainInfo(string domainId);

    Task<DomainDto> UpdateDomain(string domainId, DomainUpdateDto updateDto, string userId);
    Task<bool> DeleteDomain(string domainId, string userId);
    Task<StaticResourceRelatedResult<PostDto>> UpdatePost(string postId, PostUpdateDto updateDto, string userId);
    Task<StaticResourceRelatedResult<bool>> DeletePost(string postId, string userId);

    Task<DomainCategoryDto> AddDomainCategory(string domainId, DomainCategoryUpdateDto updateDto,
        string userId);

    Task<DomainCategoryDto> UpdateDomainCategory(string domainCategoryId,
        DomainCategoryUpdateDto updateDto, string userId);

    Task<bool> DeleteDomainCategoryImmediately(string domainCategoryId, string userId);

    Task<DomainTopicDto> AddDomainTopic(string domainId, DomainTopicUpdateDto updateDto, string userId);

    Task<DomainTopicDto> UpdateDomainTopic(string domainTopicId,
        DomainTopicUpdateDto updateDto, string userId);

    Task<bool> DeleteDomainTopicImmediately(string domainTopicId, string userId);
}