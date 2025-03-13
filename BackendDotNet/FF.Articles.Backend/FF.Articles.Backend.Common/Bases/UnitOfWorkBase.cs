using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public abstract class UnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : DbContext
{
    private readonly TContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
    {
        try
        {
            await BeginTransactionAsync();
            var result = await action();
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
            Dispose();
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        try
        {
            await BeginTransactionAsync();
            await action();
            await CommitAsync();
        }
        catch (Exception)
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            Dispose();
        }
    }
    public IDbContextTransaction BeginTransaction() => _context.Database.BeginTransaction();

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();

    public void Commit()
    {
        try
        {
            _transaction?.Commit();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Rollback()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
        finally
        {
            if (_transaction != null)
                await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}