using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
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

  //public async Task<(IEnumerable<Game>, PaginationMetadataDto)> GetAllAsync(bool sort=false, int pageNr = 1, int pageSize = 20)
    public async Task<(IEnumerable<Game>, PaginationMetadataDto)> GetAllAsync(GameRequestParams gParams)
    {
        var filter = context.Game.AsQueryable<Game>();
        if (!string.IsNullOrEmpty(gParams.Title))
            filter = filter.Where(o => o.Title.Equals(gParams.Title));
       

        var total = await filter.CountAsync();
        var xxx = new PaginationMetadataDto(total, gParams.PageSize, gParams.PageNumber);

        return (gParams.Sort ? await filter.OrderBy(o => o.Title)
                    .Skip(gParams.PageSize * (gParams.PageNumber - 1) )
                    .Take(gParams.PageSize)
                    .ToListAsync() 
                    : await filter
                    .Skip(gParams.PageSize * (gParams.PageNumber - 1))
                    .Take(gParams.PageSize)
                    .ToListAsync(), xxx );
    }

    public async Task<Game> GetAsync(int id)
    {
        return await context.Game.SingleOrDefaultAsync(o => o.Id == id);
    }
    //public async Task<(IEnumerable<Game>,PaginationMetadataDto)> GetByTitleAsync(string title, int pageNr = 1, int pageSize = 20)
    //{
    //    var filter = context.Game.Where(o => o.Title == title);

    //    var total = await filter.CountAsync();
    //    var pg = new PaginationMetadataDto(total, pageSize, pageNr);

    //    var data = await filter
    //        .Skip(pageSize * (pageNr - 1))
    //        .Take(pageSize)
    //        .ToListAsync();

    //    return (data, pg);
    //    //return await context.Game.Where(o => o.Title == title).ToListAsync();
    //}


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
