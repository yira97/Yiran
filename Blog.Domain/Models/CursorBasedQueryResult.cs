namespace Blog.Domain.Models;

public class CursorBasedQueryResult<T>
{
    public string NextPage { get; set; } = string.Empty;
    public string PreviousPage { get; set; } = string.Empty;
    public bool HasNext { get; set; } = true;
    
    public bool HasPrevious { get; set; } = false;
    public List<T> Data { get; set; } = new();
}