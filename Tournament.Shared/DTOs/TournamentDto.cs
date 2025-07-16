using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Shared.DTOs;

public record TournamentDto(string Title, DateTime StartDate)
{
    public DateTime EndDate { get; init; } = StartDate.AddMonths(3);
    public IEnumerable<GameDto> Games { get; init; }

}
