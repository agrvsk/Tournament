using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class TournamentRepository(TournamentContext context) : ITournamentRepository
{

    public void Add(TournamentDetails tournament)
    {
        context.TournamentDetails.Add(tournament);
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await context.TournamentDetails.AnyAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<TournamentDetails>> GetAllAsync()
    {
        return await context.TournamentDetails.ToArrayAsync();
    }

    public async Task<TournamentDetails> GetAsync(int id)
    {
        return await context.TournamentDetails.SingleOrDefaultAsync(x => x.Id == id);
    }

    public void Remove(TournamentDetails tournament)
    {
        context.TournamentDetails.Remove(tournament);
    }

    public void Update(TournamentDetails tournament)
    {
        context.TournamentDetails.Update(tournament);
    }
}
