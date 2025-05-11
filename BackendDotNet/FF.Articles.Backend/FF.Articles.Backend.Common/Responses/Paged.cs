namespace FF.Articles.Backend.Common.Responses;

public class Paged<T> where T : class
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public int Counts { get; set; }

    public List<T> Data { get; set; }
    public Paged() { Data = new List<T>(); }

    public Paged(int pageIndex, int pageSize, int count, List<T> data)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Counts = count;
        Data = data;
    }
    public Paged((int pageIndex, int pageSize, int count) pageInfo)
    {
        PageIndex = pageInfo.pageIndex;
        PageSize = pageInfo.pageSize;
        Counts = pageInfo.count;
        Data = new List<T>();
    }
    /// <summary>
    ///  var res = new Paged<T>(Ts.GetPageInfo(), newData);
    /// </summary>
    public Paged((int pageIndex, int pageSize, int count) pageInfo, List<T> data)
    {
        PageIndex = pageInfo.pageIndex;
        PageSize = pageInfo.pageSize;
        Counts = pageInfo.count;
        Data = data;
    }
    public (int pageIndex, int pageSize, int count) GetPageInfo() => (PageIndex, PageSize, Counts);

}