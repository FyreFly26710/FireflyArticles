// namespace FF.Articles.Backend.Common.BackgoundJobs;

// [Obsolete("This is not used anymore, only use redis for caching")]
// public enum ChangeType
// {
//     Create,
//     Update,
//     Delete
// }

// [Obsolete("This is not used anymore, only use redis for caching")]
// public class DataChange
// {
//     public string FullName { get; set; } = default!;
//     public string EntityType { get; set; } = default!;
//     public long Id { get; set; }
//     public ChangeType ChangeType { get; set; }
//     // Create and Update need payload
//     public string? PayloadJson { get; set; }
//     public DateTime Timestamp { get; set; } = DateTime.UtcNow;
// }
