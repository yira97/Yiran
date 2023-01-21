namespace Blog.Domain.Models;

public record AccessTokenDto(
    string? AccessToken,
    string? RefreshToken
);