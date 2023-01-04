namespace Blog.Domain.Models;

public class StaticResourceRelatedResult<T>
{
    public T Data { get; set; }
    public List<CategoryResource> Added { get; set; } = new();
    public List<string> Removed { get; set; } = new();
}