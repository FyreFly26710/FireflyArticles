using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;

namespace FF.Articles.Backend.AI.API.UnitOfWork;

public interface IAIDbContextUnitOfWork : IUnitOfWork<AIDbContext>
{

}
