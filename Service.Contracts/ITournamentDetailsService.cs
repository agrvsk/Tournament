using Microsoft.AspNetCore.JsonPatch;
using Tournament.Shared.DTOs;
using Tournament.Core.Responses;
using Tournament.Shared.Requests;

namespace Service.Contracts;

public interface ITournamentDetailsService
{
    //Task<ResultObjectDto<IEnumerable<TournamentDto>>> GetAllAsync(bool showGames = false, bool sorted = false, int pageNr = 1, int pageSize = 20);
    //Task<ResultObjectDto<IEnumerable<TournamentDto>>> GetAllAsync(TournamentRequestParams tParams);
    Task<ApiBaseResponse> GetAllAsync(TournamentRequestParams tParams);
    Task<ApiBaseResponse> GetAsync(int id, bool showGames=false);
    Task<ApiBaseResponse> CreateAsync(TournamentCreateDto create);
    Task<ApiBaseResponse> UpdateAsync(TournamentUpdateDto update);
    Task<ApiBaseResponse> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument);
    Task<ApiBaseResponse> DeleteAsync(int id);
}