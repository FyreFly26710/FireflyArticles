using FF.Articles.Backend.Common.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Bases;
public interface IBaseService<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<List<TEntity>> GetByIdsAsync(List<int> ids);
    Task<List<TEntity>> GetAllAsync();
    Task<Paged<TEntity>> GetAllAsync(PageRequest pageRequest);
    Task<int> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(int id);
}