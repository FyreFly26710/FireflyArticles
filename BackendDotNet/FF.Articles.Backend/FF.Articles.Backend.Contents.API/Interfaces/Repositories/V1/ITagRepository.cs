namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories.V1;
public interface ITagRepository : IBaseRepository<Tag, ContentsDbContext>
{
    Task<List<Tag>> GetByNamesAsync(List<string> names);
    Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names);
}



