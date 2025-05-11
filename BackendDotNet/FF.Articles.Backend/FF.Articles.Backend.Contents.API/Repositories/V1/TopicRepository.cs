namespace FF.Articles.Backend.Contents.API.Repositories.V1;
public class TopicRepository : BaseRepository<Topic, ContentsDbContext>, ITopicRepository
{
    public TopicRepository(ContentsDbContext _context) : base(_context)
    {
    }

}
