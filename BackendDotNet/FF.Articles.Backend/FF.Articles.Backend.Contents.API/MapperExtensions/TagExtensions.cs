namespace FF.Articles.Backend.Contents.API.MapperExtensions;
public static class TagExtensions
{
    public static TagDto ToDto(this Tag tag)
    {
        var tagDto = new TagDto
        {
            TagId = tag.Id,
            TagName = tag.TagName,
            TagGroup = tag.TagGroup,
            TagColour = tag.TagColour
        };
        return tagDto;
    }
}