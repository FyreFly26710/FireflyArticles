public interface IUnitOfWork<TContext>
    where TContext : DbContext
{

    //Task<IDbContextTransaction> BeginTransactionAsync();
    //Task CommitAsync();
    //Task RollbackAsync();
    //Task<int> SaveChangesAsync();
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);
    Task ExecuteInTransactionAsync(Func<Task> action);
}