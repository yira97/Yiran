namespace Blog.Admin.Models;

public record NavigationDto(
    string Name,
    string Href,
    bool Current
);