namespace Blog.Domain.Models;

public class CursorBasedQueryResult<T>
{
    public string NextPage { get; set; } = string.Empty;
    public string PreviousPage { get; set; } = string.Empty;
    public bool HasMore { get; set; } = true;
    public List<T> Data { get; set; } = new();
}