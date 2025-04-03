using FF.Articles.Backend.AI.API.Infrastructure;
using FF.Articles.Backend.AI.API.Interfaces.Repositories;

namespace FF.Articles.Backend.AI.API.UnitOfWork;

public class UnitOfWork(AIDbContext context) : UnitOfWork<AIDbContext>(context), IAIDbContextUnitOfWork
{

}
