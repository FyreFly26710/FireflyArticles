using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories;
public interface ITagRepository : IBaseRepository<Tag, ContentsDbContext>
{
    Task<List<Tag>> GetByNamesAsync(List<string> names);
    Task<List<Tag>> GetOrCreateByNamesAsync(List<string> names);
}



