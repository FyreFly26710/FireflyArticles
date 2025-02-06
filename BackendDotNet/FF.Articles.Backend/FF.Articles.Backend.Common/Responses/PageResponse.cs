using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Responses;
public class PageResponse<TEntity>(int pageIndex, int pageSize, long count, List<TEntity> data) where TEntity : class
{
    public int PageIndex { get; } = pageIndex;

    public int PageSize { get; } = pageSize;

    public long Count { get; } = count;

    public List<TEntity> Data { get; } = data;
}