namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V2;

public interface ITagRedisRepository : IRedisRepository<Tag>
{
    Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names);
}
