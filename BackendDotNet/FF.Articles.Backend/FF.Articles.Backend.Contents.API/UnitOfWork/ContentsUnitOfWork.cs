namespace FF.Articles.Backend.Contents.API.UnitOfWork;
public class ContentsUnitOfWork(ContentsDbContext context) : UnitOfWork<ContentsDbContext>(context), IContentsUnitOfWork
{
}
