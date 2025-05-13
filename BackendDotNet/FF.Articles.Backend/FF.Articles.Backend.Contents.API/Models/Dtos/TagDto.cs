namespace FF.Articles.Backend.Contents.API.Models.Dtos;
public class TagDto
{
    public long TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string? TagGroup { get; set; }
    public string? TagColour { get; set; }

}
