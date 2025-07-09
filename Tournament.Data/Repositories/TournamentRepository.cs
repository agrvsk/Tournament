using System;
using System.Collections.Generic;
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

    //public async Task<(IEnumerable<TournamentDetails>,PaginationMetadataDto)> GetAllAsync(bool showGames = false, bool sort = false, int pageNr = 1, int pageSize = 20)
     public async Task<(IEnumerable<TournamentDetails>, PaginationMetadataDto)> GetAllAsync(TournamentRequestParams tParam)
    {
        IQueryable<TournamentDetails> data = context.TournamentDetails;
        var total = await data.CountAsync();
        var pg = new PaginationMetadataDto(total, tParam.PageSize, tParam.PageNumber);

        if (tParam.Sort)
            data = data.OrderBy(x => x.Title);

        data = data
            .Skip(tParam.PageSize * (tParam.PageNumber - 1))
            .Take(tParam.PageSize);

        if (tParam.ShowGames)
            data = data.Include(x => x.Games);

        return (await data.ToListAsync(), pg);

        //return showGames ? SortFunc( await context.TournamentDetails.Include(c => c.Games ).ToArrayAsync(),sort)
        //                 : SortFunc( await context.TournamentDetails.ToArrayAsync(), sort);
    }

    public IEnumerable<TournamentDetails> SortFunc(IEnumerable<TournamentDetails>  items, bool Sort)
    {
        return Sort
        ? items.OrderBy(x => x.Title)
        : items;
    }





    public async Task<TournamentDetails> GetAsync(int id, bool showGames = false)
    {
        return showGames ? await context.TournamentDetails.Include(c => c.Games).SingleOrDefaultAsync(x => x.Id == id)
                         : await context.TournamentDetails.SingleOrDefaultAsync(x => x.Id == id);
    }

    public void Remove(TournamentDetails tournament)
    {
        context.TournamentDetails.Remove(tournament);
    }

    //public async Task UpdateAsync(TournamentDetails tournament)
    //{
    //    TournamentDetails org = await GetAsync(tournament.Id);
    //    org.Id = tournament.Id;
    //    org.StartDate = tournament.StartDate;
    //    org.Title = tournament.Title;
    //    //context.TournamentDetails.Update(tournament);
    //}

    public void Update(TournamentDetails tournament)
    {
        context.TournamentDetails.Update(tournament);
    }

    public void SetStateModified(TournamentDetails? tournament)
    {
        if(tournament != null)
        context.Entry(tournament).State = EntityState.Modified;
    }
}
