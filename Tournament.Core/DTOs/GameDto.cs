using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public record GameDto   //(string Title, DateTime StartDate)
{
    public string Title { get; init; }
    public DateTime StartDate { get; init; }

}
