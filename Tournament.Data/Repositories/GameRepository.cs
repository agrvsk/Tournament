﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories;

public class GameRepository(TournamentContext context) : IGameRepository
{
    public void Add(Game game)
    {
        context.Game.Add(game);
    }

    public async Task<bool> AnyAsync(int id)
    {
        return await context.Game.AnyAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Game>> GetAllAsync(bool sort=false)
    {
        return sort ? await context.Game.OrderBy(o => o.Title).ToListAsync() 
                    : await context.Game.ToListAsync();
    }

    public async Task<Game> GetAsync(int id)
    {
        return await context.Game.SingleOrDefaultAsync(o => o.Id == id);
    }
    public async Task<IEnumerable<Game>> GetByTitleAsync(string title)
    {
        return await context.Game.Where(o => o.Title == title).ToListAsync();
    }


    public void Remove(Game game)
    {
        context.Game.Remove(game);
    }
    //TODO
    //public async Task UpdateAsync(Game game)
    //{
    //    Game org = await GetAsync(game.Id);
    //    org.Id = game.Id;
    //    org.Title = game.Title;
    //    org.Time = game.Time;
    //    org.TournamentDetailsId = game.TournamentDetailsId;
    //    //context.Game.Update(game);
    //}

    public void Update(Game game)
    {
        context.Game.Update(game);
    }

    public void SetStateModified(Game? game)
    {
        if(game != null)
        context.Entry(game).State = EntityState.Modified;
    }
    

}
