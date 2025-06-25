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

public class TournamentUoW (TournamentContext _context, 
    ITournamentRepository tournamentRepository,
    IGameRepository gameRepository) 
    : ITournamentUoW
{
    public ITournamentRepository TournamentRepository => tournamentRepository;
    public IGameRepository GameRepository => gameRepository;

    //private readonly TournamentContext _context;
    //public ITournamentRepository TournamentRepository { get; }
    //public IGameRepository GameRepository { get; }
    //public TournamentUoW(TournamentContext context)
    //{
    //    _context = context;
    //    TournamentRepository = new TournamentRepository(context);
    //    GameRepository = new GameRepository(context);
    //}


    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }


}
