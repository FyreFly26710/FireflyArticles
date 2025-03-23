using System;

namespace FF.Articles.Backend.Common.BackgoundJobs;

public enum ChangeType
{
    Create,
    Update,
    Delete
}

public class DataChange
{
    public string FullName { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public int Id { get; set; }
    public ChangeType ChangeType { get; set; }
    // Create and Update need payload
    public string? PayloadJson { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
