using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public record ResultObject(bool IsSuccess, string Message, int StatusCode, int Id, PaginationMetadataDto pagination)
{

}
