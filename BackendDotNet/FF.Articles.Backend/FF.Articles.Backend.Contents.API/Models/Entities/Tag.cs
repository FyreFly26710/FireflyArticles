namespace FF.Articles.Backend.Contents.API.Models.Entities;
/// <summary>
/// Ignore BaseEntity optional columns
/// </summary>
public class Tag : BaseEntity
{
    public string TagName { get; set; }
    public string? TagGroup { get; set; }
    public string? TagColour { get; set; }
}
