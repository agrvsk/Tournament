using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public class TournamentCreateDto() //string Title, DateTime StartDate
{
    public string Title { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; } // = StartDate.AddMonths(3);
}
