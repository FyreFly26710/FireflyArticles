using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Responses;
//public class PageResponse<T>(int pageIndex, int pageSize, int count, List<T> data) where T : class
//{
//    public int PageIndex { get; } = pageIndex;

//    public int PageSize { get; } = pageSize;

//    public int Count { get; } = count;

//    public List<T> Data { get; } = data;
//}
public class PageResponse<T> where T : class
{
    public PageResponse()
    {
        Data = new List<T>();
    }

    public PageResponse(int pageIndex, int pageSize, int count, List<T> data)
    { 
        PageIndex = pageIndex;
        PageSize = pageSize;
        RecordCount = count;
        Data = data;
    }
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int RecordCount { get; set; } 

    public List<T> Data { get; set; }
}