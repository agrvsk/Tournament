﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public class ResultObjectDto<T>
{
    //public ResultObject(bool IsSuccess, string Message, int StatusCode, int Id, T? Data, PaginationMetadataDto? Pagination)
    //{
    //    Value1 = value1;
    //    Value2 = value2;
    //    Value3 = value3;
    //    Value4 = value4;
    //    Value5 = value5;
    //}

    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public int Id { get; set; }
    public T? Data { get; set; }
    public PaginationMetadataDto Pagination { get; set; }
}
