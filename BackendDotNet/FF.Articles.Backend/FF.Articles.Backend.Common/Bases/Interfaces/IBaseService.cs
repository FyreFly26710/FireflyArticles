namespace FF.Articles.Backend.Common.Bases.Interfaces;

public interface IBaseService<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(long id);
    Task<List<TEntity>> GetByIdsAsync(List<long> ids);
    Task<List<TEntity>> GetAllAsync();
    Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest);
    Task<long> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(long id);
}