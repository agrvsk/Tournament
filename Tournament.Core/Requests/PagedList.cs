using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Requests;

public class PagedList<T>
{
    public IReadOnlyList<T> Items { get;}
    public MetaData MetaData { get; set; }

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        (
            CurrentPage: pageNumber,
//          TotalPages: (int)Math.Ceiling(count / (double) pageSize),
            PageSize: pageSize,
            TotalCount: count
        );

        Items = new List<T>(items).AsReadOnly();
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

}
