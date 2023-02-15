namespace Blog.Api.Entities;

public class TextContentTranslationEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string TextContentId { get; set; } = string.Empty;

    public string Translation { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;
}