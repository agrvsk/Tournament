using Tournament.Core.DTOs;

namespace Service.Contracts;

public interface ITournamentDetailsService
{
    Task<IEnumerable<TournamentDto>>  GetAllAsync(bool showGames=false, bool sorted=false);

    Task<TournamentDto> GetAsync(int id, bool showGames=false);
}