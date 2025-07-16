using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Shared.DTOs;
using Tournament.Core.Entities;
using Tournament.Shared.Requests;

namespace Tournament.Core.Repositories;

public interface IGameRepository
{
    Task<PagedList<Game>> GetAllAsync(GameRequestParams gParams);
    Task<Game> GetAsync(int id);
    Task<bool> AnyAsync(int id);
    Task<int> GetGameCount(int TournamentId);

    void Add(Game game);
    void Update(Game game);
    void Remove(Game game);

    void SetStateModified(Game game);
}
