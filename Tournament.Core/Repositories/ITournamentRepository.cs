using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Requests;

namespace Tournament.Core.Repositories;

public interface ITournamentRepository
{
    Task<PagedList<TournamentDetails>> GetAllAsync(TournamentRequestParams tParams);
    Task<TournamentDetails> GetAsync(int id, bool showGames=false);
    Task<bool> AnyAsync(int id);
    void Add(TournamentDetails tournament);
    void Update(TournamentDetails tournament);
    void Remove(TournamentDetails tournament);
    public void SetStateModified(TournamentDetails tournament);
}
