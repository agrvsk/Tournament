﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public record GameDto
{
    public string Title { get; init; } 
    public DateTime Time { get; init; } 

}
