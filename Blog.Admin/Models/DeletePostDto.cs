namespace Blog.Admin.Models;

public class DeletePostDto
{
    public string DomainId { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
}