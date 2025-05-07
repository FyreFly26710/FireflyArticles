using FF.Articles.Backend.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF.Articles.Backend.Common.Responses;
public class PageRequest
{
    /// <summary>
    /// Page number. Default: 1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size. Default: 20
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field. Default: null, override by child class
    /// </summary>
    public virtual string? SortField { get; set; }

    /// <summary>
    /// Sort order. Default: ASC
    /// </summary>
    public string SortOrder { get; set; } = SortOrderConstant.ASC;
}

