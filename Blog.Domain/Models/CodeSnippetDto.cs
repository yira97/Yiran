namespace Blog.Domain.Models;

public record CodeSnippetDto(
        string FileName,
        string Language,
        string Content
    );