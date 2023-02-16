using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IPostRepository
{
    /// <summary>
    /// 不检查 Domain 是否存在
    /// </summary>
    /// <param name="updateDto"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="BadHttpRequestException"></exception>
    StaticResourceRelatedResult<PostDto> CreatePost(PostUpdateDto updateDto, string userId);

    Task<PostDto?> GetPostInfo(string postId);

    Task<CursorBasedQueryResult<PostDto>> ListPosts(CursorBasedQuery order, ListPostFlag flag = ListPostFlag.Cover);

    Task<StaticResourceRelatedResult<PostDto>> UpdatePost(string postId, PostUpdateDto updateDto, string userId);
    Task<StaticResourceRelatedResult<bool>> DeletePost(string postId, string userId);
}