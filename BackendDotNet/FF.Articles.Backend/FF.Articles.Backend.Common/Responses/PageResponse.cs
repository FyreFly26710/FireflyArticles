using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Responses;
public class PageResponse<T>(int pageIndex, int pageSize, long count, List<T> data) where T : class
{
    public int PageIndex { get; } = pageIndex;

    public int PageSize { get; } = pageSize;

    public long Count { get; } = count;

    public List<T> Data { get; } = data;
}