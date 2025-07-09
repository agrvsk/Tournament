using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Requests;

public class MetaData(int CurrentPage,  int PageSize, int TotalCount) 
{
    public int CurrentPage { get; }
    public int TotalPages { get; } = (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    //public MetaData(int currentPage, int totalPages, int pageSize, int totalCount)
    //{
    //    CurrentPage = currentPage;
    //    TotalPages = totalPages;
    //    PageSize = pageSize;
    //    TotalCount = totalCount;
    //}
}
