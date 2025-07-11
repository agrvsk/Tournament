using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class TournamentUoW (TournamentContext _context, 
    ITournamentRepository tournamentRepository,
    IGameRepository gameRepository,
    IUserRepository userRepository) 
    : ITournamentUoW
{
    public ITournamentRepository TournamentRepository => tournamentRepository;
    public IGameRepository GameRepository => gameRepository;
    public IUserRepository UserRepository => userRepository;

    //private readonly TournamentContext _context;
    //public ITournamentRepository TournamentRepository { get; }
    //public IGameRepository GameRepository { get; }
    //public TournamentUoW(TournamentContext context)
    //{
    //    _context = context;
    //    TournamentRepository = new TournamentRepository(context);
    //    GameRepository = new GameRepository(context);
    //}


    //public async Task<ResultObjectDto<T>> CompleteAsync<T>(ResultObjectDto<T> input)
    //{
    //    input.IsSuccess = false;
    //    try
    //    {
    //        await _context.SaveChangesAsync();
    //    }
    //    catch (DbUpdateConcurrencyException e1)
    //    {
    //        input.Message = e1.Message;
    //        return input;
    //    }
    //    catch (DBConcurrencyException e2)
    //    {
    //        input.Message = e2.Message;
    //        return input;
    //    }
    //    catch (DbUpdateException e3)
    //    {
    //        input.Message = e3.Message;
    //        return input;
    //    }
    //    catch (Exception ex)
    //    {
    //        input.Message = ex.Message;
    //        return input;
    //    }

    //    input.IsSuccess = true;
    //    input.Message = string.Empty;
    //    return input;
    //}

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

}
