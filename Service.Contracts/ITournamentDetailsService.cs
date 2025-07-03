using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.DTOs;

namespace Service.Contracts;

public interface ITournamentDetailsService
{
    Task<ResultObjectDto<IEnumerable<TournamentDto>>>  GetAllAsync(bool showGames=false, bool sorted=false, int pageNr=1, int pageSize=20);
    Task<ResultObjectDto<TournamentDto>> GetAsync(int id, bool showGames=false);
    Task<ResultObjectDto<TournamentDto>> CreateAsync(TournamentCreateDto create);
    Task<ResultObjectDto<TournamentDto>> UpdateAsync(TournamentUpdateDto update);
    Task<ResultObjectDto<TournamentDto>> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument);
    Task<ResultObjectDto<int>> DeleteAsync(int id);
}