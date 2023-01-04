namespace Blog.Domain.Models;

public record EmailPasswordAuthDto(
    string Email,
    string Password
);