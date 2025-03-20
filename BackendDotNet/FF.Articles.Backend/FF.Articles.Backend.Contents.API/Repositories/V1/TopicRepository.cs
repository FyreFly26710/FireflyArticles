using FF.Articles.Backend.Common.Bases;
using FF.Articles.Backend.Contents.API.Infrastructure;
using FF.Articles.Backend.Contents.API.Interfaces.Repositories;
using FF.Articles.Backend.Contents.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FF.Articles.Backend.Contents.API.Repositories.V1;
public class TopicRepository : BaseRepository<Topic, ContentsDbContext>, ITopicRepository
{
    public TopicRepository(ContentsDbContext _context) : base(_context)
    {
    }
    public async Task<Topic?> GetTopicByTitle(string title)
    {
        return await GetQueryable()
            .FirstOrDefaultAsync(t => t.Title.Trim().ToLower() == title.Trim().ToLower());
    }

}
