namespace Blog.Api.Entities;

public class TextContentEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string OriginalText { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;
}