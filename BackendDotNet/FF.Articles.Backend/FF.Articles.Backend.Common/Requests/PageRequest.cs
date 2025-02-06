using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Requests;
public class PageRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.ASC;
}

public enum SortOrder
{
    ASC,
    DESC
}