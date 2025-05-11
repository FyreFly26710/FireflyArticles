namespace FF.Articles.Backend.Contents.API.Interfaces.Services;
public interface ITagService : IBaseService<Tag>
{
    Task<Tag?> GetTagByNameAsync(string tagName);
}

