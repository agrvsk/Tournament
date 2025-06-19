using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class TournamentUoW(TournamentContext context, 
    ITournamentRepository TournamentRepository,
    IGameRepository GameRepository) : ITournamentUoW
{
    public ITournamentRepository TournamentRepository => TournamentRepository;

    public IGameRepository GameRepository => GameRepository;

    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}
