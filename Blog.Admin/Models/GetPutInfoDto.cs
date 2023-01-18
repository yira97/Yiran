namespace Blog.Admin.Models;

public class GetPutInfoDto
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
}