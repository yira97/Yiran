using Blog.Admin.Enums;

namespace Blog.Admin.Models;
public record ActionButtonInfoDto
(
    string Name,
    string Href,
    ActionButtonType Type
);
