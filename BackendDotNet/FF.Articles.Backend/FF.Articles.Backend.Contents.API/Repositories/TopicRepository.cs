namespace FF.Articles.Backend.Contents.API.Repositories;
public class TopicRepository(ContentsDbContext _context)
    : BaseRepository<Topic, ContentsDbContext>(_context), ITopicRepository
{
}
