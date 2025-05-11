namespace FF.Articles.Backend.AI.API.UnitOfWork;

public class UnitOfWork(AIDbContext context) : UnitOfWork<AIDbContext>(context), IAIDbContextUnitOfWork
{

}
