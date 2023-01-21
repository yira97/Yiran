using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class PostViewModel
{
    public CursorBasedQueryResult<PostDto> QueryResult { get; set; } = new();
}