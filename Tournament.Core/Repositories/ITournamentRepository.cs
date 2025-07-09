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
    //Task<(IEnumerable<TournamentDetails>, PaginationMetadataDto)> GetAllAsync(bool showGames = false, bool sort = false, int pageNr = 1, int pageSize = 20);
    Task<(IEnumerable<TournamentDetails>,PaginationMetadataDto)> GetAllAsync(TournamentRequestParams tParams);
    Task<TournamentDetails> GetAsync(int id, bool showGames=false);
    Task<bool> AnyAsync(int id);
    void Add(TournamentDetails tournament);
    void Update(TournamentDetails tournament);
    void Remove(TournamentDetails tournament);
    public void SetStateModified(TournamentDetails tournament);
}
