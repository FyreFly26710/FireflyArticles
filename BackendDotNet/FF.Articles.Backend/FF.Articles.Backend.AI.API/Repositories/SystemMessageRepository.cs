namespace FF.Articles.Backend.AI.API.Repositories;

public class SystemMessageRepository : BaseRepository<SystemMessage, AIDbContext>, ISystemMessageRepository
{
    public SystemMessageRepository(AIDbContext context) : base(context)
    {
    }
}
