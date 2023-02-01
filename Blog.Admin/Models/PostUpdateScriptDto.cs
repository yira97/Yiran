using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class PostUpdateScriptDto
{
    public PostContentDto? InitialPostContentData { get; set; }
}