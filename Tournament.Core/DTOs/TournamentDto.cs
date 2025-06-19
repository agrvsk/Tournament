using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs;

public record TournamentDto(string Title, DateTime StartDate)
{
    public DateTime EndDate { get; init; } = StartDate.AddMonths(3); 
    
}
