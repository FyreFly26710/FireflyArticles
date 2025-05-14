namespace FF.Articles.Backend.Contents.API.Models.Requests.Tags;
public class TagEditRequest
{
    public long TagId { get; set; }
    public string? TagName { get; set; }
    public string? TagGroup { get; set; }
    public string? TagColour { get; set; }

}