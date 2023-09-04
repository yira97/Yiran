namespace Evrane.Profile.Api.Entities;

public class StaticResourceEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    /// <summary>
    /// 在任何情况下都不应修改Key
    /// </summary>
    public string Key { get; set; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;
    public string UpdatedById { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Action { get; set; }
    public int? Category { get; set; }
    public string? ReferenceId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime? DeletedAt { get; set; }
}