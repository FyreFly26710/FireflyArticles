using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Models.Entities;

namespace FF.Articles.Backend.Contents.API.Interfaces.Repositories;
public interface ITopicRepository : IBaseRepository<Topic, ContentsDbContext>
{
    public Task<Topic?> GetTopicByTitle(string title);
}
