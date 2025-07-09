using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Requests;

public class MetaData (int CurrentPage,  int PageSize, int TotalCount) //int TotalPages,
{
    public int CurrentPage { get; set; } = CurrentPage;
    public int PageSize { get; set; } = PageSize;
    public int TotalCount { get; set; } = TotalCount;
    public int TotalPages { get; set; } = ((int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    //public MetaData(int CurrentPage, int TotalPages, int PageSize, int TotalCount)
    //{
    //    this.CurrentPage = CurrentPage;
    //    this.TotalPages = TotalPages;
    //    this.PageSize = PageSize;
    //    this.TotalCount = TotalCount;
    //}
}
