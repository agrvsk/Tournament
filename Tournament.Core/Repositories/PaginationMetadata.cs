using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Repositories;

public class PaginationMetadata(int TotalItemCount, int PageSize, int CurrentPage)
{
    int TotalPageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize );
}
