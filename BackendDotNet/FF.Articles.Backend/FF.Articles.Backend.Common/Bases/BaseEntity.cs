namespace FF.Articles.Backend.Common.Bases;

public abstract class BaseEntity
{
    public int Id { get; set; }


    #region Optional columns
    /// <summary>
    /// BaseEntity: Create time
    /// </summary>
    public DateTime? CreateTime { get; set; }
    /// <summary>
    /// BaseEntity: Update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }
    /// <summary>
    /// BaseEntity: Is delete: 0 : false, 1: true
    /// </summary>
    public int? IsDelete { get; set; }
    #endregion

}
