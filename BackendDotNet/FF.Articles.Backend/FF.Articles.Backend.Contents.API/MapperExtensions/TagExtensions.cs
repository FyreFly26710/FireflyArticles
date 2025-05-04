using FF.Articles.Backend.Contents.API.Models.Dtos;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.MapperExtensions;
public static class TagExtensions
{
    public static TagDto ToDto(this Tag tag)
    {
        var tagDto = new TagDto
        {
            TagId = tag.Id,
            TagName = tag.TagName
        };
        return tagDto;
    }
}