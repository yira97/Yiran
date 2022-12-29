namespace Blog.Api.Models;

public class CursorBasedQuery
{
    public string PageToken { get; set; } = string.Empty;
    public int PageSize { get; set; } = 10;
    public int OrderBy { get; set; } = 0;
    public bool Ascending { get; set; } = false;
    public Dictionary<int, List<string>> Filter { get; set; } = new();
}