using Microsoft.EntityFrameworkCore.Storage;

public abstract class UnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : DbContext
{
    private readonly TContext _context;
    private IDbContextTransaction? _transaction;
    private int _transactionCount = 0; // Track nested transactions

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
    {
        bool isOuterTransaction = await BeginTransactionAsync();
        try
        {
            var result = await action();
            await SaveChangesAsync();

            if (isOuterTransaction) // Only commit if this is the outermost transaction
                await CommitAsync();

            return result;
        }
        catch (Exception)
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (isOuterTransaction)
                DisposeTransaction();
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        bool isOuterTransaction = await BeginTransactionAsync();
        try
        {
            await action();
            await SaveChangesAsync();

            if (isOuterTransaction)
                await CommitAsync();
        }
        catch (Exception)
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (isOuterTransaction)
                DisposeTransaction();
        }
    }

    private async Task<bool> BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            _transactionCount = 1; // First transaction
            return true;
        }
        else
        {
            _transactionCount++; // Nested transaction
            return false;
        }
    }

    private async Task CommitAsync()
    {
        if (_transaction != null && _transactionCount == 1)
        {
            await _transaction.CommitAsync();
        }
        _transactionCount--;
    }

    private async Task RollbackAsync()
    {
        if (_transaction != null && _transactionCount == 1)
        {
            await _transaction.RollbackAsync();
        }
        _transactionCount--;
    }

    private void DisposeTransaction()
    {
        if (_transaction != null && _transactionCount <= 0)
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    private async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

}
