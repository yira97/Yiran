using Blog.Domain.Models;

namespace Blog.Admin.Models;

public class TopicViewModel
{
    public List<DomainTopicDto> Topics { get; set; } = new();
}